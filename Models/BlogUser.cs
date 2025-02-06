using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
