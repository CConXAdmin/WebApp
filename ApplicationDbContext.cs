using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1
{
    public interface ITenantService
    {
        public int GetTenantId();
        public int GetLoggedInUserId();
    }
    public class TenantService : ITenantService
    {
        public int GetTenantId() { return 1; }
        public int GetLoggedInUserId() { return 1; }
    }
    public class ApplicationDbContext : IdentityDbContext
    {
        public int TenantId;
        public int LoggedInUserId;
        private readonly ITenantService _tenantService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
        {
            _tenantService = tenantService;
            TenantId = _tenantService.GetTenantId();
            LoggedInUserId = _tenantService.GetLoggedInUserId();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //CANT FILTER BEFORE CANVIEW VALUE SET
            //modelBuilder.Entity<BaseItem>().HasQueryFilter(x => x.canView == true);
 
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
            foreach (var entry in ChangeTracker.Entries<hasTenant>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.TenantId = TenantId;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<hasUser>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.CreatedbyUserId = TenantId;
                        break;
                }
            }
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        public DbSet<TestItem1> TestItem1s { get; set; } = default;
        public DbSet<TestItem2> TestItem2s { get; set; } = default;

        public IEnumerable<TestItem1> GetTestItem1s() {
            return TestItem1s.AsEnumerable().Where(x => x.canView == true); ;
        }
        public IEnumerable<TestItem2> GetTestItem2s()
        {
            return TestItem2s.AsEnumerable().Where(x => x.canView == true); ;
        } 
    }
    public enum Sharedwith
    {
        Public,
        Private,
        Tenant,
        Archive
    }
    public interface hasTenant
    {
        public int TenantId { get; set; }
    }
    public interface hasUser
    {
        public int CreatedbyUserId { get; set; }
    }
    public class BaseItem : hasTenant, hasUser
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Description { get; set; }
        public int CreatedbyUserId { get; set; }
        public Sharedwith Sharedwith { get; set; }
        [NotMapped]
        public bool canView
        {
            get
            { 
                return Extentions.CheckifCanView(this.TenantId, this.CreatedbyUserId, this.Sharedwith);
            }
            set { }
        } 
    }
    public class TestItem1 : BaseItem, hasTenant, hasUser
    {
        public string CustomProp { get; set; }
    }
    public class TestItem2 : BaseItem, hasTenant, hasUser
    {
        public string CustomProp { get; set; }
    }
    public static class Extentions
    {
        //CANT USE THIS IN STATIC CLASS
        //readonly ApplicationDbContext _context;
        //public Extentions(ApplicationDbContext _context) {
        //    _context = _context;
        //}
        public static bool CheckifCanView(int TenantId, int CreatedbyUserId, Sharedwith Sharedwith)
        {
            //CANT USE THIS IN STATIC CLASS
            //var _context = new ApplicationDbContext();
            //var c = _context.TenantId;
            
            var contextTenantid = 1; //var contextTenantid = _context.TenantId;
            var contextUsertid = 1; //var contextUsertid = _context.LoggedInUserId;

            switch (Sharedwith)
            {
                case Sharedwith.Public: return true; break;
                case Sharedwith.Private: return CreatedbyUserId==contextUsertid; break;
                case Sharedwith.Tenant: return TenantId==contextTenantid; break;
                case Sharedwith.Archive: return false; break;
                default: return false;
            }
        }
    }
}