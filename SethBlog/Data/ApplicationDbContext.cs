using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SethBlog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SethBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<BlogUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SethBlog.Models.Blog> Blog { get; set; }
        public DbSet<SethBlog.Models.Post> Post { get; set; }
        public DbSet<SethBlog.Models.Comment> Comment { get; set; }
    }
}
