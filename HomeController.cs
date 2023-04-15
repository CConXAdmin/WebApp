using Microsoft.AspNetCore.Mvc;

namespace WebApplication1
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model2 = _context.GetTestItem1s(_context);

            var model1 = _context.TestItem1s.FindTheLast(x => x.Id, 2);
            var model = _context.TestItem1s.canView(_context).AsEnumerable(); 


            return View(model);
        }
        public IActionResult Create()
        { 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenantId,CreatedbyUserId,Description, CustomProp, Sharedwith")] TestItem1 model)
        {
            _context.Add(model); 
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Index2()
        {
            var model = _context.TestItem2s.canView(_context);
            return View(model.ToList());
        }
        public IActionResult Create2()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create2([Bind("TenantId,CreatedbyUserId,Description, CustomProp, Sharedwith")] TestItem2 model)
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index2");
        }
    }
}
