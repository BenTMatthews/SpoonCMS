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
