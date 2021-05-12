using SethBlog.Data;
using SethBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }
        public IQueryable<Post> SearchContent(string searchString)
        {
            //Use Linq to search all posts
            var result = _context.Post.Where(p => p.PostState == Enums.PostState.Published);
            //if no user input, return ALL posts
            if (string.IsNullOrEmpty(searchString))
            {
                return result;
            }
            //narrow down results
            result = result.Where(p => p.Title.Contains(searchString) ||
                                      p.Abstract.Contains(searchString) ||
                                      p.Content.Contains(searchString) ||
                                      //If comment has been moderated, don't search regular body.
                                      p.Comments.Any(c => c.Moderated == null && c.Body.Contains(searchString) ||
                                                         c.ModeratedBody.Contains(searchString) ||
                                                         c.Author.FirstName.Contains(searchString) ||
                                                         c.Author.LastName.Contains(searchString) ||
                                                         c.Author.Email.Contains(searchString)));
            //possible email removal due to privacy in an extreme edge case. 
            return result;
        }
    }
}
