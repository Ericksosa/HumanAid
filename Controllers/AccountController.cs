using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanAid.Controllers
{
    public class AccountController : Controller
    {
        private readonly HumanAidDbContext _context;

        public AccountController(HumanAidDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(String Correo, string Contraseña)
        {
            var Usuario = _context.Usuario.Include(u => u.Rol).FirstOrDefault(u => u.Correo == Correo && u.Clave == Contraseña);
            if (Usuario != null)
            {
                switch (Usuario.Rol.Nombre)
                {
                    case "Administrador":
                        return RedirectToAction("Index", "Admin");
                    case "Socio":
                        return RedirectToAction("Index", "Socio");
                    case "VoluntarioSanitario":
                        return RedirectToAction("Index", "VoluntarioSanitario");
                    case "VoluntarioAdministrativo":
                        return RedirectToAction("Index", "VoluntarioAdministrativo");

                }
            }
            ViewBag.Mensaje = "Correo o contraseña incorrectos.";
            return View();
        }


    }
}
