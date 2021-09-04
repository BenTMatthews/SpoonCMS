using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ExampleCore3._0.Models;
using SpoonCMSCore.Interfaces;

namespace ExampleCore3._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpoonData _spoonData;

        public HomeController(ILogger<HomeController> logger, ISpoonData spoonData)
        {
            _logger = logger;
            _spoonData = spoonData;
        }

        public IActionResult Index()
        {
            _spoonData.GetContainer(1);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
