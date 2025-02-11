﻿using SethBlog.Enums;
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

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters. ", MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters. ", MinimumLength = 2)]
        public string Abstract { get; set; }
        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        public string Slug { get; set; } //Derived from title (will be programatically implemented) 
        //give myself ability to record state of any post
        //i.e. ready, published, draft
        [Display(Name ="Post State")]
        public PostState PostState { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Published Date")]
        public DateTime? PublishedDate { get; set; } 
        [Display(Name ="Estimated Minutes To Read")]
        public string ReadTime { get; set; }
        //image data
        [Display(Name ="Post Image")]
        public byte[] PostImage { get; set; }
        public string ContentType { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
        //navigational property
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual Blog Blog { get; set; }
    }
}
