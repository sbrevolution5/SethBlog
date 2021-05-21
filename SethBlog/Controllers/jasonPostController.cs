//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SethBlog.Controllers
//{
//    public class jasonPostController : Controller
//    {
//        public IActionResult Index()
//        {
//            var originalPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);
//            var newSlug = _slugService.UrlFriendly(post.Title);
//            if (newSlug != originalPost.Slug)
//            {
//                if (_slugService.IsUnique(newSlug))
//                {
//                    originalPost.Title = post.Title;
//                    originalPost.Slug = newSlug;
//                }
//                else
//                {
//                    ModelState.AddModelError("Title", "This Title produces a non-unique Slug and cannot be used");
//                    return View(post);
//                }
//            }
//            _context.Tags.RemoveRange(originalPost.Tags);

//            foreach (var tag in tagValues)
//            {
//                _context.Add(new Tag()
//                {
//                    PostId = post.Id,
//                    AuthorId = originalPost.AuthorId,
//                    Text = tag
//                });
//            }
//            if (newImage != null)
//            {
//                originalPost.ImageData = await _imageService.EncodeImageAsync(newImage);
//                originalPost.ContentType = _imageService.ContentType(newImage);
//            }
//            originalPost.Abstract = post.Abstract;
//            originalPost.Content = post.Content;
//            originalPost.ReadyStatus = post.ReadyStatus;
//            originalPost.Updated = DateTime.Now;
//            await _context.SaveChangesAsync();
//            return View();
//        }

//    }
//}
