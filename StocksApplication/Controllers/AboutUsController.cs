using Microsoft.AspNetCore.Mvc;

namespace StocksApplication.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
