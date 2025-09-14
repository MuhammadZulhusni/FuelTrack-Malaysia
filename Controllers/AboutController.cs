using Microsoft.AspNetCore.Mvc;

namespace assignment1.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/About/AboutUs.cshtml");
        }
    }
}