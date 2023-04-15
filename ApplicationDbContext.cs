using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

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
    public interface IUserService
    {
        public bool GetUserCanView(int TenantId, int CreatedbyUserId, Sharedwith Sharedwith);
    }
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context; 
        private HttpContext _httpContext;
        public UserService(ApplicationDbContext context,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _httpContext = contextAccessor.HttpContext;
        }
        public bool GetUserCanView(int TenantId, int CreatedbyUserId, Sharedwith Sharedwith)
        {
            //CANT USE THIS IN STATIC CLASS
            //var _context = new ApplicationDbContext();
            //var c = _context.TenantId;

            var contextTenantid = 1; //var contextTenantid = _context.TenantId;
            var contextUsertid = 1; //var contextUsertid = _context.LoggedInUserId;

            switch (Sharedwith)
            {
                case Sharedwith.Public: return true; break;
                case Sharedwith.Private: return CreatedbyUserId == contextUsertid; break;
                case Sharedwith.Tenant: return TenantId == contextTenantid; break;
                case Sharedwith.Archive: return false; break;
                default: return false;
            }
        }
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

        public IEnumerable<TestItem1> GetTestItem1s(ApplicationDbContext _context) {
            return TestItem1s.AsEnumerable();
            return TestItem1s.AsEnumerable().Where(x => x.canView == true);
        }
        public IEnumerable<TestItem2> GetTestItem2s()
        {
            return TestItem2s;
            return TestItem2s.AsEnumerable().Where(x => x.canView == true);
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
        private ApplicationDbContext _context;
        public BaseItem(ApplicationDbContext context) { 
            _context = context;
        }
        public BaseItem() { }
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Description { get; set; }
        public int CreatedbyUserId { get; set; }
        public Sharedwith Sharedwith { get; set; }
        [NotMapped]
        public bool canView { get; set; }
        //{
        //    get
        //    {
 
        //        //return Extentions.CheckifCanView(this.TenantId, this.CreatedbyUserId, this.Sharedwith, _context);
        //    }
        //    set { }
        //} 
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
        public static bool CheckifCanView(int TenantId, int CreatedbyUserId, Sharedwith Sharedwith, ApplicationDbContext _context)
        { 
            var contextTenantid = _context.TenantId;
            var contextUsertid = _context.LoggedInUserId; 

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
    public abstract class Extentions2
    {
        public ApplicationDbContext _context; 
        public HttpContext _httpContext;
        public Extentions2(ApplicationDbContext context, IHttpContextAccessor contextAccessor) {
            _context = context;
            _httpContext = contextAccessor.HttpContext;
        }

        public bool CheckifCanView(int TenantId, int CreatedbyUserId, Sharedwith Sharedwith)
        {
            //CANT USE THIS IN STATIC CLASS
            //var _context = new ApplicationDbContext();
            //var c = _context.TenantId;

            var contextTenantid = _context.TenantId;
            var contextUsertid = _context.LoggedInUserId;
             

            switch (Sharedwith)
            {
                case Sharedwith.Public: return true; break;
                case Sharedwith.Private: return CreatedbyUserId == contextUsertid; break;
                case Sharedwith.Tenant: return TenantId == contextTenantid; break;
                case Sharedwith.Archive: return false; break;
                default: return false;
            }
        }
    }
    public static class DbSetExtensions
    {
        public static IQueryable<T> FindTheLast<T, TResult>(this IQueryable<T> t, Expression<Func<T, TResult>> expression, int nums) where T : class
        {
            return t.OrderByDescending(expression).Take(nums);
        }
        public static IQueryable<T> canView<T>(this IQueryable<T> t,   ApplicationDbContext _context) where T : BaseItem
        {
            var newt = new List<T>();
            foreach (var item in t)
            {
                item.canView = Extentions.CheckifCanView(item.TenantId, item.CreatedbyUserId, item.Sharedwith, _context);
                if(item.canView==true) newt.Add(item);
            }
             
            return newt.AsQueryable();
        }
        public static IQueryable<T> cannotView<T>(this IQueryable<T> t,   ApplicationDbContext _context) where T : BaseItem
        {
            var newt = new List<T>();
            foreach (var item in t)
            {
                item.canView = Extentions.CheckifCanView(item.TenantId, item.CreatedbyUserId, item.Sharedwith, _context);
                if (item.canView == false) newt.Add(item);
            }

            return newt.AsQueryable();
        }
    }
}