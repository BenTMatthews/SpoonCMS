using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    public class ContentItem : IItem
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public bool Active { get; set; } = true;
        public DateTime BeginDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public DateTime Created { get; } = DateTime.Now;
        public String Name { get; set; }

        object IItem.Value
        {
            get { return this.Value; }
            set { this.Value = (string)value; }
        }

        public ContentItem()
        {

        }

        public ContentItem(string name)
        {
            Name = name;
        }
    }
}
