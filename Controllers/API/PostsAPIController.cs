using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SethBlog.Data;
using SethBlog.Models;

namespace SethBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostsAPI
        /// <summary>
        /// Gets a number of most recent posts from the blog
        /// </summary>
        /// <param name="num">Number of posts to retrieve</param>
        /// <returns>An array of Post objects</returns>
        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetTopXPosts(int num)
        {
            return await _context.Post.OrderByDescending(p=>p.PublishedDate).Take(num).ToListAsync();
        }

        // GET: api/PostsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Post.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        //// PUT: api/PostsAPI/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutPost(int id, Post post)
        //{
        //    if (id != post.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(post).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!PostExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/PostsAPI
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Post>> PostPost(Post post)
        //{
        //    _context.Post.Add(post);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetPost", new { id = post.Id }, post);
        //}

        // DELETE: api/PostsAPI/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletePost(int id)
        //{
        //    var post = await _context.Post.FindAsync(id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Post.Remove(post);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
