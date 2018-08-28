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
        public bool Active { get; set; } 
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; } 
        public DateTime Created { get; } 
        public String Name { get; set; }
        public int Priority { get; set; }

        public ContentItem()
        {
            Id = Guid.NewGuid();
            Active = true;  
            BeginDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
            Created = DateTime.Now;
            Priority = 0;
        }

        public ContentItem(string name) : this()
        {
            Name = name;
        }
    }
}
