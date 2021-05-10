using SethBlog.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{

    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string ModeratorId { get; set; }
        [Required]
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Moderated { get; set; }
        /// <summary>
        /// What the moderater changed the body to
        /// </summary>
        public string ModeratedBody { get; set; }
        public ModerationReason ModerationReason { get; set; }
        public virtual BlogUser Moderator { get; set; }
        public virtual Post Post { get; set; }
        public virtual BlogUser Author { get; set; }
    }
}
