using SpoonCMS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleCore.SpoonCMSExtensions
{
    public static class ContainerExtensions
    {
        public static string GetTitle(this Container container)
        {
            return container.GetItem("pageTitle").Value;
        }
    }
}
