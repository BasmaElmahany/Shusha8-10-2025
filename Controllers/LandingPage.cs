using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shusha_project_BackUp.Controllers
{
    public class LandingPage : Controller
    {
        // GET: LandingPage
        public ActionResult Index()
        {
            return View();
        }

    
        public IActionResult Details()
        {
            return View();
        }
    }
}
