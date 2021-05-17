using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SethBlog.Data;
using SethBlog.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SethBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            //handles case of no page given, in that case give it the first page.
            var pageNumber = page ?? 1;
            var pageSize = 5;
            // Get latest post? maybe use viewdata?
            var latestPost = await _context.Post.OrderByDescending(p => p.PublishedDate).FirstAsync();
            ViewData["latestPost"] = latestPost; //TODO This isn't quite right
            var allBlogs = await _context.Blog
                .OrderByDescending(b=>b.Created)
                .Include(b => b.Posts
                    .Where(p => p.PostState == Enums.PostState.Published)
                    .OrderByDescending(p => p.PublishedDate))
                .ToPagedListAsync(pageNumber,pageSize);
            return View(allBlogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
