using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    class Container
    {
        public int Id { get; set; }
        public List<IItem> Items {get; set;}
        public bool Active { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        private DateTime Created { get; set; }
        public String Name { get; set; }
    }
}
