using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;


namespace ExampleCore.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return Content(SpoonCMS.Workers.SpoonWebWorker.BuildAdminPageString(), "text/html");
        }

    }
}
