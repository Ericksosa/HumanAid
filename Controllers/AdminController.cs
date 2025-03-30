using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumanAid.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }  
       
    }
}
