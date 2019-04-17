using ExampleCore.Models;
using ExampleCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using SpoonCMSCore.Classes;
using SpoonCMSCore.Interfaces;
using System.Diagnostics;

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
            HomePageViewModel viewModel = new HomePageViewModel();
            Container container = _spoonData.GetContainer("HomePage");

            viewModel.rows = container.GetItem("rows").Value;

            ViewData["Title"] = container.GetItem("pageTitle").Value;
            viewModel.carousel = container.GetItem("myCarousel").Value;

            return View(viewModel);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
