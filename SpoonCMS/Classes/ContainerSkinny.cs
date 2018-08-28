using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    public class ContainerSkinny
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public String Name { get; set; }

        public ContainerSkinny()
        {
            Created = DateTime.Now;
            Active = true;
        }
    }
}
