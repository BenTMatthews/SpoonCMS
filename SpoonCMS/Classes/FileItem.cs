using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpoonCMS.Classes
{
    public class FileItem : IItem
    {
        public int Id { get; set; }
        public Stream Value { get; set; }
        public bool Active { get; set; } = true;
        public DateTime BeginDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public DateTime Created { get; } = DateTime.Now;
        public String Name { get; set; }

        object IItem.Value
        {
            get { return this.Value; }
            set { this.Value = (Stream)value; }
        }


        public FileItem(string name)
        {
            Name = name;
        }
    }
}
