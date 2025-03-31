using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
