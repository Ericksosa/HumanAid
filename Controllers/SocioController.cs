using Microsoft.AspNetCore.Mvc;

namespace HumanAid.Controllers
{
    public class SocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
