using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SpoonCMS.Workers;
using SpoonCMS.Interfaces;
using SpoonCMS.Classes;

namespace ExampleCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpoonData _spoonData;

        public HomeController(ISpoonData spoonData)
        {
            _spoonData = spoonData;
        }
        public IActionResult Index()
        {
            Container con = _spoonData.GetContainer("HomePage");
            ViewData["rows"] = con.GetItem("rows").Value;
            ViewData["Title"] = con.GetItem("pageTitle").Value;
            ViewData["Carousel"] = con.GetItem("myCarousel").Value;
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
