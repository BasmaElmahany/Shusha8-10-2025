using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shusha_project_BackUp.Models;

namespace Shusha_project_BackUp.Controllers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class FeedStockController : Controller
    {
        private ApplicationDbContext _context;
        public FeedStockController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var pqty = _context.feedInventories.Sum(s => s.Tons_qty);
            var bqty = _context.FeedPurchases.Sum(s => s.Tons_qty);
            var total = pqty + bqty;
            var vm = new FeedStockVm
            {
                feedStock = total
            };

            return View(vm);
        }
       
    }
}
