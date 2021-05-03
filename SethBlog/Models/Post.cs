using SethBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string Slug { get; set; } //Derived from title (will be programatically implemented) 
        //give myself ability to record state of any post
        //i.e. ready, published, draft
        [Display(Name ="Publish State")]
        public PostState PostState { get; set; }

    }
}
