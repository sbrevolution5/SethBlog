using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters. ", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters. ", MinimumLength = 2)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; } //nullable because it will initially be null
        //Todo: Most recent blog post date

        public byte[] BlogImage { get; set; }
        public string ContentType { get; set; }
        //navigational properties
        //Virtual allows for lazy loading (instead of eager)
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        [NotMapped]
        public DateTime LatestPostDate
        {
            get
            {
                if (Posts.Count > 0)
                {

                    return Posts.OrderByDescending(p => p.Created).FirstOrDefault().Created;
                }
                else
                {
                    return Created;
                }
            }
        }
    }
}
