using Microsoft.AspNetCore.Mvc;

namespace HumanAid.Areas.Administrador.Controllers
{
    public class SocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
