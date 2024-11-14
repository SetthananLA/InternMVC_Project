using InternProjectMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace InternProjectMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
