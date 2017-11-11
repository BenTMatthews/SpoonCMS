using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;


namespace ExampleCore.Controllers
{
    public class SpoonController : Controller
    {
        public IActionResult Admin()
        {
            return Content(SpoonCMS.Workers.SpoonWebWorker.BuildAdminPageString(), "text/html");
        }

    }
}
