using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SpoonCMS.Workers;

namespace ExampleCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = SpoonDataWorker.GetContainer("HomePage").GetItem("pageTitle").Value;
            ViewData["Carousel"] = SpoonDataWorker.GetContainer("HomePage").GetItem("myCarousel").Value;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
