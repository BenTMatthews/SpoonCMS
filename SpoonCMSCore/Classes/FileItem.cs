using SpoonCMSCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpoonCMSCore.Classes
{
    public class FileItem : IItem
    {
        public Guid Id { get; set; }
        public Stream Value { get; set; }
        public bool Active { get; set; } 
        public DateTime BeginDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public DateTime Created { get; } 
        public String Name { get; set; }

        object IItem.Value
        {
            get {
                return this.Value;
            }
            set {
                this.Value = (Stream)value;
            }
        }

        public FileItem()
        {
            Id = Guid.NewGuid();
            Active = true;
            Created = DateTime.Now;
            BeginDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
        }

        public FileItem(string name) : this()
        {
            Name = name;
        }
    }
}
