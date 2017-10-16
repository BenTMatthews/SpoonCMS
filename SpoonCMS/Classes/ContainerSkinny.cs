using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    public class ContainerSkinny
    {
        public int Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime Created { get; set; } = DateTime.Now;
        public String Name { get; set; }
    }
}
