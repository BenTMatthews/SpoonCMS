using SpoonCMS.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Interfaces
{
    interface ISpoonData
    {
        string ConnString { get; set; }

        Container GetContainer(string conName);
        void AddContainer(Container container);
        void DeleteContainer(string conName);
        void AddItemToContainer(string conName, IItem item);
        void UpdateItemInContainer(string conName, IItem item);
        void DeleteItemInContainer(string conName, string itemName);
    }
}
