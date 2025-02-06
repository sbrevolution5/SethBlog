using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SethBlog.Data;
using SethBlog.Models;
using SethBlog.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Connection.GetConnectionString(Configuration)));
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy",
                    builder => builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod());
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddRazorPages();
            services.AddScoped<IFileService, BasicFileService>();
            services.AddScoped<DataService>();
            services.AddScoped<BasicSlugService>();
            services.AddScoped<IEmailSender, GmailEmailService>();
            services.AddScoped<SearchService>();
            services.AddScoped<ReadTimeService>();
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{Directory.GetCurrentDirectory()}/wwwroot/SethBlog.xml",true);
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Blog API",
                    Version = "v1",
                    Description = "Infusing users with data via The Coder's Codex API",
                    Contact = new OpenApiContact {
                        Email = "sethbcoding@gmail.com",
                        Name = "Seth Burleson",
                        Url = new System.Uri("https://www.linkedin.com/in/seth-a-burleson-a1555211a/")
                    }

                });
                });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("DefaultPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestAPI v1");
                c.DocumentTitle = "The Coder's Codex Public API";
                });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "slugRoute",
                    pattern: "Posts/ViewPost/{slug}",
                    defaults: new { controller = "Posts", action = "Details" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
