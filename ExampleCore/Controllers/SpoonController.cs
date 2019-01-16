using Microsoft.AspNetCore.Mvc;
using SpoonCMSCore.Workers;


namespace ExampleCore.Controllers
{
    public class SpoonController : Controller
    {
        public IActionResult Admin()
        {
            return Content(SpoonWebWorker.GenerateResponseString(Request.HttpContext, true), "text/html");
        }
    }
}
