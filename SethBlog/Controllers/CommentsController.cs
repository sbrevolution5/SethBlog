using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SethBlog.Data;
using SethBlog.Models;

namespace SethBlog.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.Author).Include(c => c.Moderator).Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Comments/Dashboard
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Dashboard()
        {
            var applicationDbContext = _context.Comments.Where(c => c.IsReviewed == false)
                .Include(c => c.Author)
                .Include(c => c.Moderator)
                .Include(c => c.Post);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Body")] Comment comment)
        {
                var post = await _context.Post.FirstOrDefaultAsync(p => p.Id == comment.PostId);
                var slug = post.Slug;
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.Now;
                comment.AuthorId = _userManager.GetUserId(User);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Posts", new { Slug = slug });

            }
            return RedirectToAction("Details", "Posts", new { Slug = slug });
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,PostId,AuthorId,ModeratorId,Body,Moderated,ModeratedBody,ModerationReason")] Comment comment, bool commentDash=false)
        {
            var post = await _context.Post.FirstOrDefaultAsync(p => p.Id == comment.PostId);
            var slug = post.Slug;
            if (id != comment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    comment.Updated = DateTime.Now;
                    if (comment.ModeratedBody is not null)
                    {
                        comment.Moderated = DateTime.Now;
                        comment.IsReviewed = true;
                        comment.ModeratorId = _userManager.GetUserId(User);

                    }
                    else
                    {
                        //TODO If user's comment is whitespace then tell them there was an error, not just redirect. 
                        // Code from drew
                        //if (!string.IsNullOrEmpty(comment.Body))
                        //{
                        //    comment.Created = DateTime.Now;
                        //    _context.Add(comment);
                        //    await _context.SaveChangesAsync();
                        comment.IsReviewed = false;
                        //}
                        return RedirectToAction("Details", "Posts", new { slug });//in case user edits a previously reviewed comment. 
                    }
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (commentDash)
                {
                    return RedirectToAction("Dashboard", "Comments");
                }
                return RedirectToAction("Details", "Posts", new { Slug = slug });
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Abstract", comment.PostId);
            return RedirectToAction("Details", "Posts", new { Slug = slug });

        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
        // GET Comments/MarkAsReviewed/id
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> MarkAsReviewed(int id, bool dash = false)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.IsReviewed = true;
            comment.ModeratorId = _userManager.GetUserId(User);
            var post = await _context.Post.FirstOrDefaultAsync(p => p.Id == comment.PostId);
            var slug = post.Slug;
            _context.Update(comment);
            await _context.SaveChangesAsync();
            if (dash)
            {
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Details", "Posts", new { Slug = slug });

        }
        // POST: Comments/Delete/5
        [Authorize(Roles = "Administrator")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            var post = await _context.Post.FirstOrDefaultAsync(p => p.Id == comment.PostId);
            var slug = post.Slug;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { Slug = slug });

        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
