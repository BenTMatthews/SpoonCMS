using Microsoft.AspNetCore.Mvc;
using SpoonCMS.Interfaces;
using SpoonCMS.Workers;


namespace ExampleCore.Controllers
{
    public class CustomController : Controller
    {
        private readonly ISpoonData _spoonData;

        public CustomController(ISpoonData spoonData)
        {
            _spoonData = spoonData;
        }

        public IActionResult Custom()
        {
            //removing leading slash
            string id = Request.Path.Value.Remove(0,1);
            if (!string.IsNullOrEmpty(id))
            {
                var container = _spoonData.GetContainer(id);

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
