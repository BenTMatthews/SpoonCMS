using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleCore.Models;
using SpoonCMS.Classes;

namespace ExampleCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            SpoonCMS.Workers.SpoonDataWorker worker = new SpoonCMS.Workers.SpoonDataWorker();

            Container container = new Container("test");

            container.AddItem(new ContentItem("test item") { Active = true, Value = "this is some HTML" });

            //worker.AddContainer(container);

            var containers = worker.GetAllContainers();

            var container2 = worker.GetContainer("test");

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
