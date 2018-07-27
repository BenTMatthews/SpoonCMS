using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SpoonCMS.Workers;
using SpoonCMS.Interfaces;
using SpoonCMS.Classes;
using ExampleCore.ViewModel;

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
            HomePageViewModel vm = new HomePageViewModel();
            Container con = _spoonData.GetContainer("HomePage");
            vm.rows = con.GetItem("rows").Value;
            ViewData["Title"] = con.GetItem("pageTitle").Value;
            vm.carousel = con.GetItem("myCarousel").Value;
            return View(vm);
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
