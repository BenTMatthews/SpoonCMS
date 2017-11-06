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

            //worker.DeleteContainer()

            Container container = new Container("test");

            container.AddItem(new ContentItem("test item") { Active = true, Value = "this is some HTML" });

            container.AddItem(new ContentItem("test item 2") { Active = true, Value = "this is also some HTML" });

            Container container2 = new Container("test2");

            container2.AddItem(new ContentItem("test item21") { Active = true, Value = "this is some HTML" });

            container2.AddItem(new ContentItem("test item22") { Active = true, Value = "this is also some HTML" });

            //worker.AddContainer(container);
            //worker.AddContainer(container2);

            var containers = worker.GetAllContainers();

            var container3 = worker.GetContainer("test2");

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
