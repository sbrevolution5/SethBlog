using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SethBlog.Data;
using SethBlog.Enums;
using SethBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IFileService fileService, IConfiguration configuration, UserManager<BlogUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _fileService = fileService;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //prevent manual update database command
            await _context.Database.MigrateAsync();
            //Task 1: Seed roles (create and enter into Authorization system
            await SeedRolesAsync();
            // Task 2 seed a few users into AspNetUsers
            await SeedUsersAsync();
        }
        private async Task SeedRolesAsync()
        {
            //Are there roles in the system? 
            if (_context.Roles.Any())
            {
                return;
            }
            //Spin through enum and do stuff
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                //create Role in system for each role
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
        private async Task SeedUsersAsync()
        {
            if (_context.Users.Count() > 2)
            {
                return;
            }
            var modUser = new BlogUser()
            {
                Email = "sbrevolution5.sb@gmail.com",
                UserName = "sbrevolution5.sb@gmail.com",
                FirstName = "Seth",
                LastName = "Burleson",
                PhoneNumber = "919-7634059",
                EmailConfirmed = true,
                ImageData = await _fileService.EncodeFileAsync(_configuration["DefaultUserImage"]),
                ContentType = "png",
            };
            await _userManager.CreateAsync(modUser, _configuration["ModPassword"]);
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
            var adminUser = new BlogUser()
            {
                Email = "sethbcoding@gmail.com",
                UserName = "sethbcoding@gmail.com",
                FirstName = "Seth",
                LastName = "Burleson",
                PhoneNumber = "919-763-4059",
                EmailConfirmed = true, //Has to be forced for the admin
                ImageData = await _fileService.EncodeFileAsync(_configuration["DefaultUserImage"]),
                ContentType = "png",
            };
            await _userManager.CreateAsync(adminUser, _configuration["AdminPassword"]);
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());
            
        }
    }
}
