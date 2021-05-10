using System;
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
using SethBlog.Models;
using SethBlog.Services;

namespace SethBlog.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration; 

        public PostsController(ApplicationDbContext context, IFileService fileService, IConfiguration configuration)
        {
            _context = context;
            _fileService = fileService;
            _configuration = configuration;
        }
        //GET: Posts of one blog
        [AllowAnonymous]
        public async Task<ActionResult> BlogPostIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["BlogName"] = _context.Blog.First(b => b.Id == id).Name;
            var blogPosts = await _context.Post.Where(p => p.BlogId == id).ToListAsync();
            return View(blogPosts);
        }
        // GET: All Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Post.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)//Acts on the previous include
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            //Syntax: (All data, One column [choosing], other column[for display])
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadTime,PostState")] Post post, IFormFile customFile)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;
                post.PostImage = (await _fileService.EncodeFileAsync(customFile)) ?? await _fileService.EncodeFileAsync(_configuration["DefaultBlogImage"]);
                post.ContentType = customFile is null ? _configuration["DefaultUserImage"].Split('.')[1] : _fileService.RecordContentType(customFile);
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
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

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Description", post.BlogId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,PostImage,ContentType,Abstract,Content,Created,PostState,ReadTime")] Post post,IFormFile NewImage)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Updated = DateTime.Now;
                    if (NewImage is not null)
                    {
                        post.PostImage = await _fileService.EncodeFileAsync(NewImage);
                        post.ContentType = _fileService.RecordContentType(NewImage);
                    }
                    _context.Update(post);
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
                return RedirectToAction("BlogPostIndex", new { id = post.BlogId });
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Description", post.BlogId);
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
