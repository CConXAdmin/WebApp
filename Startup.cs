using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; 

namespace WebApplication1
{ 
    public class Startup
    {
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration; 
        }
         
        public IConfiguration Configuration { get; } 
        public void ConfigureServices(IServiceCollection services)
        { 
            var connectionString = Configuration.GetConnectionString("DefaultConnection"); 
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddControllers();
            services.AddRazorPages();
            services.AddMvc();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)); 
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            app.UseRouting(); 
            app.UseAuthentication();
            app.UseAuthorization();  
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
                endpoints.MapControllerRoute(
                    name: "Areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
 
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        } 
    } 
}
