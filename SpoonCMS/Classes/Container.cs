using Newtonsoft.Json;
using SpoonCMS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpoonCMS.Classes
{
    public class Container
    {
        private const int _maxCount = 100;

        public int Id { get; set; }

        [JsonProperty("Items")]
        private Dictionary<string, ContentItem> _items { get; set; } = new Dictionary<string, ContentItem>();

        [JsonIgnore]
        public Dictionary<string, ContentItem> Items
        {
            get
            {
                return this._items.OrderBy(x => x.Value.Priority).ToDictionary(x => x.Key, x => x.Value);
            }
            set
            {
                _items = value;
            }
        } 
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

        /// <summary>
        /// Add an item to the container
        /// </summary>
        /// <param name="item">Content Item to add</param>
        public void AddItem(ContentItem item)
        {
            try
            {
                if (_items.Count >= _maxCount)
                {
                    throw new CountExceededException("Containers cannot exceed " + _maxCount + " items");
                }

                _items.Add(item.Name, item);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get an item from the container that matches the specified name
        /// </summary>
        /// <param name="itemName">Name of the item to fetch</param>
        /// <returns>Item matching the name provided</returns>
        public ContentItem GetItem(string itemName)
        {
            return _items[itemName];
        }

        /// <summary>
        /// Get the first item stored in the collection
        /// </summary>
        /// <returns>first item stored in the collection</returns>
        public ContentItem GetItem()
        {
            return Items.First().Value;
        }

        /// <summary>
        /// Get an item from the container that matches the specified name
        /// </summary>
        /// <param name="itemName">>Name of the item to fetch</param>
        /// <returns>Item matching the name provided</returns>
        //public ContentItem this[string itemName]
        //{
        //    get
        //    {
        //        return GetItem(itemName);
        //    }
        //    set
        //    {
        //        SetItem(itemName, value);
        //    }
        //}

        /// <summary>
        /// Set the value for the specified item
        /// </summary>
        /// <param name="itemName">Name of the item to update</param>
        /// <param name="item">The new value for the item</param>
        public void SetItem(string itemName, ContentItem item)
        {
            _items[itemName] = item;
        }

        /// <summary>
        /// Remove an item of the specified name from the collection
        /// </summary>
        /// <param name="itemName">Name of the item to remove</param>
        public void RemoveItem(string itemName)
        {
            _items.Remove(itemName);
        }

        /// <summary>
        /// Return a ContainerSkinny instance based on the current container
        /// </summary>
        /// <returns>ContainerSkinny instance based on the current container</returns>
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
