using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess;
using DataAccess.Data;
using DataAccess.Entities;
using DataAccess.Interfaces;
using Social_Network.Hubs;

namespace Social_Network
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
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IAsyncRepository<Chat>, BaseRepository<Chat>>();
            services.AddTransient<IAsyncRepository<Message>, BaseRepository<Message>>();
            services.AddTransient<IAsyncRepository<UsersInChats>, BaseRepository<UsersInChats>>();
            services.AddTransient<IAsyncRepository<User>, BaseRepository<User>>();
            services.AddTransient<IAsyncRepository<Friendship>, BaseRepository<Friendship>>();
            services.AddTransient<IAsyncRepository<Post>, BaseRepository<Post>>();
            services.AddTransient<IAsyncRepository<PostImages>, BaseRepository<PostImages>>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IChatService,ChatService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFriendshipService, FriendshipService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IRolesService, RolesService>();
            services.AddTransient<IImagesService, ImagesService>();
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddSignalR();
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
