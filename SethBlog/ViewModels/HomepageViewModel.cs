using SethBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SethBlog.ViewModels
{
    public class HomepageViewModel
    {
        public Post LatestPost { get; set; }
        public IPagedList<Blog> Blogs { get; set; }
        public List<String> TagNames { get; set; }
        public List<int> TagCounts{ get; set; }
    }
}
