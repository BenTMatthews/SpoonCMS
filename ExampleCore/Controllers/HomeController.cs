using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SpoonCMS.Workers;
using SpoonCMS.Interfaces;
using SpoonCMS.Classes;
using ExampleCore.ViewModel;
using ExampleCore.SpoonCMSExtensions;

namespace ExampleCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpoonData _spoonData;

        public HomeController(ISpoonData spoonData)
        {
            _spoonData = spoonData;
        }

        [HttpGet("{container}")]
        public IActionResult Index(string container)
        {
            //TODO: If !container redirect to this action for homepage
            //HomePageViewModel viewModel = new HomePageViewModel();
            Container _container = _spoonData.GetContainer(container);

            var title = _container.GetTitle();
            SetTitle(title);
            SetMenuItems();

            var items = _container.GetAllItems();
            return View(items);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetTitle(string title)
        {
            ViewBag.title = title;
        }
        private void SetMenuItems()
        {
            var menuItems = _spoonData.GetAllContainers().Select(x => x.Name).ToArray();
            ViewBag.menuItems = menuItems;
        }
    }
}
