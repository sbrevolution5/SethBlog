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
        public IOrderedQueryable<Post> SearchContent(string searchString)
        {
            //TODO Potential Improvements
            // Non-case sensitive search
            // ignoring formatting (HTML) when searching.
            // splitting words, adding to the end of our search, I.E. showing a later result for How to use divs in HTML, if the user searches for How to format divs in CSS.  The wording is slightly different, but there is still some relevance as it does include divs.  
                //If we include the above feature, Maybe ignore words shorter than 3 characters, and the word "The".
            //Use Linq to search all posts
            var result = _context.Post.Where(p => p.PostState == Enums.PostState.Published);
            //if no user input, return ALL posts
            if (!string.IsNullOrEmpty(searchString))
            {
            
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
            }
            //possible email removal due to privacy in an extreme edge case. 
            return result.OrderByDescending(p => p.Created);
        }
    }
}
