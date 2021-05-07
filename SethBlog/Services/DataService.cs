using Microsoft.AspNetCore.Identity;
using SethBlog.Data;
using SethBlog.Enums;
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

        public DataService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task ManageDataAsync()
        {
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

        }
    }
}
