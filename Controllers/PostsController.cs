﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SethBlog.Data;
using SethBlog.Enums;
using SethBlog.Models;
using SethBlog.Services;
using X.PagedList;

namespace SethBlog.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly BasicSlugService _slugService;
        private readonly SearchService _searchService;
        private readonly ReadTimeService _readTimeService;

        public PostsController(ApplicationDbContext context, IFileService fileService, IConfiguration configuration, BasicSlugService slugService, SearchService searchService, ReadTimeService readTimeService)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
            _slugService = slugService;
            _searchService = searchService;
            _readTimeService = readTimeService;
        }
        //GET:All Posts of one blog
        [AllowAnonymous]
        public async Task<ActionResult> BlogPostIndex(int? id, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            if (id == null)
            {
                return NotFound();
            }
            ViewData["BlogName"] = _context.Blog.First(b => b.Id == id).Name;
            ViewData["BlogId"] = id;

            if (!User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
            {
                //If user is not in authorized roles, do include unpublished
                var blogPosts = await _context.Post.Where(p => p.BlogId == id && p.PostState == PostState.Published)
                    .Include(p => p.Comments)
                    .Include(p => p.Blog)
                    .OrderByDescending(p => p.PublishedDate ?? p.Created)
                    .ToPagedListAsync(pageNumber, pageSize);
                return View(blogPosts);

            }
            else
            {
                var blogPosts = await _context.Post.Where(p => p.BlogId == id)
                    .Include(p => p.Comments)
                    .OrderByDescending(p=> p.PublishedDate ?? p.Created)
                    .ToPagedListAsync(pageNumber, pageSize);

            return View(blogPosts);
            }
            
        }//GET:All Posts 
        [AllowAnonymous]
        public async Task<ActionResult> PostIndex(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            //if (id == null)
            //{
            //    return NotFound();
            //}
            //ViewData["BlogName"] = _context.Blog.First(b => b.Id == id).Name;
            //ViewData["BlogId"] = id;

            if (!User.IsInRole("Administrator") && !User.IsInRole("Moderator"))
            {
                //If user is not in authorized roles, do include unpublished
                var allPosts = await _context.Post.Where(p => p.PostState == PostState.Published)
                    .Include(p => p.Comments)
                    .Include(p => p.Blog) // needed to display blog title
                    .OrderByDescending(p => p.PublishedDate ?? p.Created)
                    .ToPagedListAsync(pageNumber, pageSize);
                return View(allPosts);

            }
            else
            {
                var allPosts = await _context.Post.Where(p => p.Id == p.Id) //gets ALL elements
                    .Include(p => p.Comments)
                    .Include(p => p.Blog)
                    .OrderByDescending(p => p.PublishedDate ?? p.Created)
                    .ToPagedListAsync(pageNumber, pageSize);

                return View(allPosts);
            }
            
        }
        // GET: All Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Post.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }
        //Get: Search results
        //[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(string searchString, int? page) 
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            ViewData["SearchString"] = searchString;
            // get results from our search service.
            var posts = await _searchService.SearchContent(searchString).ToPagedListAsync(pageNumber,pageSize);
            //posts = await posts.ToListAsync();
            return View(posts);
        }
        //GET: Tag index
        [AllowAnonymous]
        public async Task<IActionResult> TagIndex(string tagText, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            ViewData["TagText"] = tagText;
            tagText = tagText.ToLower();
            var allPostIds = _context.Tags.Where(t => t.Text.ToLower() == tagText).Select(t => t.PostId);
            var posts = await _context.Post.Where(p => allPostIds.Contains(p.Id)).Where(p => p.PostState == Enums.PostState.Published).OrderByDescending(p => p.PublishedDate).ToPagedListAsync(pageNumber, pageSize);
            
            return View(posts);
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string slug, int? fromPage)//Takes slug, not id
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }
            ViewData["FromPage"] = fromPage;

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)//Acts on the previous include
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create(int? blogId)
        {
            var post = new Post();
            if (blogId is null)
            {
                //Syntax: (All data, One column [choosing], other column[for display])
                ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name");

            }
            else
            {
                post.BlogId = (int)blogId;
                post.Blog = _context.Blog.Find(post.BlogId);
            }
            return View(post);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,PostState")] Post post, IFormFile customFile, List<string> TagValues)
        {
            if (ModelState.IsValid)
            {
                post.ReadTime = _readTimeService.CalcReadTime(post.Content);
                post.Created = DateTime.Now;
                post.PostImage = (await _fileService.EncodeFileAsync(customFile)) ?? await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);
                post.ContentType = customFile is null ? _configuration["DefaultUserImage"].Split('.')[1] : _fileService.RecordContentType(customFile);
                if (post.PostState == PostState.Published)
                {
                    post.PublishedDate = DateTime.Now;
                    var blog = _context.Blog.First(b => b.Id == post.BlogId);
                    blog.LatestPostDate = post.PublishedDate;
                }
                //Slug stuff
                var slug = _slugService.UrlFriendly(post.Title);
                if (_slugService.IsUnique(slug))
                {
                    post.Slug = slug;
                }
                else //if there is already a slug with that name: The user must change title.
                {
                    ModelState.AddModelError("Title", "Your title is not unique enough, there is another title that is similar.");
                    ModelState.AddModelError("", "Your title is not unique enough, there is another title that is similar.");
                    ViewData["TagValues"] = string.Join(",", TagValues);
                    return View(post);
                }
                var tagList = new List<Tag>();
                foreach (var tag in TagValues)
                {
                    var newTag = new Tag()
                    {
                        PostId = post.Id,
                        Text = tag
                    };
                    _context.Add(newTag);
                    tagList.Add(newTag);
                }
                post.Tags = tagList;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
            ViewData["TagValues"] = string.Join(",", TagValues);
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(p => p.Tags).FirstOrDefaultAsync(p=> p.Id == id);

            if (post == null)
            {
                return NotFound();
            }
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Created,Slug,PostImage,ContentType,Title,Abstract,PublishedDate,Content,PostState")] Post post, IFormFile NewImage, List<string> TagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //new tracked "original post\
                    var originalPost = await _context.Post.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    originalPost.ReadTime = _readTimeService.CalcReadTime(originalPost.Content);
                    originalPost.Updated = DateTime.Now;
                    // Handles published date and updates parent blog's LatestPostDate
                    if (post.PostState == PostState.Published && post.PublishedDate == null)
                    {
                        originalPost.PublishedDate = DateTime.Now;
                        var blog = _context.Blog.First(b => b.Id == originalPost.BlogId);
                        blog.LatestPostDate = originalPost.PublishedDate;
                    }
                    else
                    {
                        originalPost.PublishedDate = post.PublishedDate;
                        
                    }
                    if (NewImage is not null)
                    {
                        originalPost.PostImage = await _fileService.EncodeFileAsync(NewImage);
                        originalPost.ContentType = _fileService.RecordContentType(NewImage);
                    }
                    else
                    {
                        originalPost.PostImage = post.PostImage;
                        originalPost.ContentType = post.ContentType;
                    }

                    var newSlug = _slugService.UrlFriendly(post.Title);
                    if (originalPost.Slug != newSlug)
                    {

                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "Your title is not unique enough, there is another title that is similar.");
                            ViewData["TagValues"] = string.Join(",", TagValues);
                            return View(post);
                        }
                        originalPost.Slug = newSlug;
                    }
                    _context.Tags.RemoveRange(originalPost.Tags);


                    foreach (var tag in TagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            Text = tag
                        });
                    }
                    originalPost.Created = post.Created;
                    originalPost.Title = post.Title;
                    originalPost.Abstract = post.Abstract;
                    originalPost.Content = post.Content;
                    originalPost.PostState = post.PostState;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { slug = post.Slug});
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name", post.BlogId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
