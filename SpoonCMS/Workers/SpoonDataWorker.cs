using LiteDB;
using SpoonCMS.Classes;
using SpoonCMS.DataClasses;
using SpoonCMS.Exceptions;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;

namespace SpoonCMS.Workers
{
    public class SpoonDataWorker
    {
        private ISpoonData data;

        #region Contructors
        public SpoonDataWorker()
        {
            data = new LiteDBData();
        }

        public SpoonDataWorker(string connString)
        {
            data = new LiteDBData(connString);
        }

        //public Spoon(DBSettings settings)
        //{

        //}

        #endregion

        #region DataOperations

        public void AddContainer(Container con)
        {
            data.AddContainer(con);
        }

        public void AddItemToContainer(string conName, IItem item)
        {
            data.AddItemToContainer(conName, item);
        }

        public void DeleteContainer(string conName)
        {
            data.DeleteContainer(conName);
        }

        public void UpdateItemInContainer(string conName, IItem item)
        {
            data.UpdateItemInContainer(conName, item);
        }

        public void DeleteItemInContainer(string conName, string itemName)
        {
            data.DeleteItemInContainer(conName, itemName);
        }

        public IItem GetItemByName(string conName, string itemName)
        {
            Container con = data.GetContainer(conName);
            if (con.Items.ContainsKey(itemName))
            {
                return con.Items[itemName];
            }
            else
            {
                throw new ItemDoesNotExistException("The Item with the specified name does not exist.");
            }
        }

        public Container GetContainer(string conName)
        {
            return data.GetContainer(conName);
        }

        public List<ContainerSkinny> GetAllContainers()
        {
            return data.GetAllContainers();
        }

        #endregion

    }
}
