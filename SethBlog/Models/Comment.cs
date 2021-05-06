using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
    }
}
