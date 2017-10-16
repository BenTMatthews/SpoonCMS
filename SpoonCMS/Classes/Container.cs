using SpoonCMS.Exceptions;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpoonCMS.Classes
{
    public class Container
    {
        private const int _maxCount = 100;

        public int Id { get; set; }
        public Dictionary<string, IItem> Items { get; set; } = new Dictionary<string, IItem>();
        public bool Active { get; set; } = true;
        public DateTime Created { get; } = DateTime.Now;
        public String Name { get; set; }

        public Container()
        {

        }

        public Container(string name)
        {
            Name = name;
        }

        public void AddItem(IItem item)
        {
            try
            {
                if(Items.Count >= _maxCount)
                {
                    throw new CountExceededException("Containers cannot exceed " + _maxCount + " items");
                }

                Items.Add(item.Name, item);
            }
            catch(ArgumentException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveItem(string itemName)
        {
            Items.Remove(itemName);
        }

        public ContainerSkinny GetSkinny()
        {
            return new ContainerSkinny
            {
                Id = this.Id,
                Active = this.Active,
                Created = this.Created,
                Name = this.Name
            };
        }
    }
}
