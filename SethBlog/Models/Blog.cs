using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; } //nullable because it will initially be null
    }
}
