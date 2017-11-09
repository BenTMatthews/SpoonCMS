using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    public class ContentItem 
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public bool Active { get; set; } = true;
        public DateTime BeginDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public DateTime Created { get; } = DateTime.Now;
        public String Name { get; set; }
        public int Priority { get; set; } = 0;

        public ContentItem()
        {
            Id = Guid.NewGuid();
        }

        public ContentItem(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
