using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration; 

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build(); 
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("app");
                try
                { 
                    var db = services.GetRequiredService<ApplicationDbContext>();
                    await Seeds(db); 
                    logger.LogInformation("Finished Seeding Default Data in Program.cs");
                    logger.LogInformation("Application Starting");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "An error occurred seeding the DB");
                }
            }
            host.Run();
        }

        private static async Task Seeds(ApplicationDbContext db)
        {
            db.TestItem1s.Add(new TestItem1 { TenantId = 1, CreatedbyUserId = 1, Description = "Public item created by me", CustomProp = "A", Sharedwith = Sharedwith.Public });
            db.TestItem1s.Add(new TestItem1 { TenantId = 1, CreatedbyUserId = 2, Description = "Public item by someone else", CustomProp = "A", Sharedwith = Sharedwith.Public });
            db.TestItem1s.Add(new TestItem1 { TenantId = 3, CreatedbyUserId = 3, Description = "Private item by someone else", CustomProp = "Should not see", Sharedwith = Sharedwith.Private });  
            db.TestItem1s.Add(new TestItem1 { TenantId = 1, CreatedbyUserId = 1, Description = "Private item by me", CustomProp = "A", Sharedwith = Sharedwith.Private });
            db.TestItem1s.Add(new TestItem1 { TenantId = 1, CreatedbyUserId = 2, Description = "Tenant item my tenant", CustomProp = "A", Sharedwith = Sharedwith.Tenant });
            db.TestItem1s.Add(new TestItem1 { TenantId = 2, CreatedbyUserId = 2, Description = "Tenant item other tenant", CustomProp = "Should not see", Sharedwith = Sharedwith.Tenant });
            db.TestItem1s.Add(new TestItem1 { TenantId = 3, CreatedbyUserId = 3, Description = "Archive item", CustomProp = "Should not see", Sharedwith = Sharedwith.Archive });

            db.TestItem2s.Add(new TestItem2 { TenantId = 1, CreatedbyUserId = 1, Description = "Public item by me", CustomProp = "A", Sharedwith = Sharedwith.Public });
            db.TestItem2s.Add(new TestItem2 { TenantId = 2, CreatedbyUserId = 3, Description = "Public item by dif tenant", CustomProp = "A", Sharedwith = Sharedwith.Public });
            db.TestItem2s.Add(new TestItem2 { TenantId = 1, CreatedbyUserId = 1, Description = "Private item by me", CustomProp = "A", Sharedwith = Sharedwith.Private });
            db.TestItem2s.Add(new TestItem2 { TenantId = 2, CreatedbyUserId = 2, Description = "Private item by someone else", CustomProp = "Should not see", Sharedwith = Sharedwith.Private });
            db.TestItem2s.Add(new TestItem2 { TenantId = 1, CreatedbyUserId = 2, Description = "Tenant item my tenant", CustomProp = "A", Sharedwith = Sharedwith.Tenant });
            db.TestItem2s.Add(new TestItem2 { TenantId = 2, CreatedbyUserId = 3, Description = "Tenant item other tenant", CustomProp = "Should not see", Sharedwith = Sharedwith.Tenant });
            db.TestItem2s.Add(new TestItem2 { TenantId = 1, CreatedbyUserId = 1, Description = "Archive item", CustomProp = "Should not see", Sharedwith = Sharedwith.Archive });
            await db.SaveChangesAsync();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) 
                .ConfigureWebHostDefaults(webBuilder =>
                { 
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIIS();
                });
    }
}