using Microsoft.AspNetCore.Mvc;

namespace ShortLink.Web.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
