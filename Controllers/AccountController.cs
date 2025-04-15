using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<IActionResult> Login(string Correo, string Contraseña)
        {
            var usuario = await _context.Usuario.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Correo == Correo && u.Clave == Contraseña);
            if (usuario != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()) // Add user ID to claims
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                switch (usuario.Rol.Nombre)
                {
                    case "Administrador":
                        return RedirectToAction("Index", "Home", new { area = "Administrador" });
                    case "Socio":
                        return RedirectToAction("Index", "Home", new { area = "Socio" });
                    case "VoluntarioSanitario":
                        return RedirectToAction("Index", "Home", new { area = "VoluntariosSanitarios" });
                    case "VoluntarioAdministrativo":
                        return RedirectToAction("Index", "Home", new { area = "VoluntariosAdministrativos" });
                }
            }

            ViewBag.Mensaje = "Correo o contraseña incorrectos.";
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

