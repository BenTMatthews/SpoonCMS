using Microsoft.AspNetCore.Mvc;
using SpoonCMSCore.Workers;
using System.Threading.Tasks;

namespace ExampleCore.Controllers
{
    public class SpoonController : Controller
    {
        public async Task<IActionResult> Admin()
        {
            return Content(await SpoonWebWorker.GenerateResponseString(Request.HttpContext, true), "text/html");
        }
    }
}
