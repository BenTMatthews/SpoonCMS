using Microsoft.AspNetCore.Mvc;
using SpoonCMS.Workers;


namespace ExampleCore.Controllers
{
    public class CustomController : Controller
    {
        public IActionResult Custom()
        {
            //removing leading slash
            string id = Request.Path.Value.Remove(0,1);
            if (!string.IsNullOrEmpty(id))
            {
                var container = SpoonDataWorker.GetContainer(id);

                if (container != null && container.Items.Count > 0)
                {
                    ViewData["CustomData"] = container.GetItem().Value;
                    return View("Custom");
                }
            }

            return View("NotFound");
        }

    }
}
