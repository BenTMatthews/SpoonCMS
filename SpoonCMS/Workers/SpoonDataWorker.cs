using LiteDB;
using SpoonCMS.Classes;
using SpoonCMS.DataClasses;
using SpoonCMS.Exceptions;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;

namespace SpoonCMS.Workers
{
    public static class SpoonDataWorker
    {
        private static ISpoonData data;
        public static string connString { get; set; } = "";

        #region Contructors

        static SpoonDataWorker()
        {
            data = new LiteDBData(connString);
        }

        #endregion

        #region DataOperations

        public static void AddContainer(Container con)
        {
            data.AddContainer(con);
        }

        public static void AddItemToContainer(string conName, ContentItem item)
        {
            data.AddItemToContainer(conName, item);
        }

        public static void DeleteContainer(string conName)
        {
            data.DeleteContainer(conName);
        }

        public static void UpdateItemInContainer(string conName, ContentItem item)
        {
            data.UpdateItemInContainer(conName, item);
        }

        public static void UpdateContainer(Container con)
        {
            data.UpdateContainer(con);
        }

        public static void DeleteItemInContainer(string conName, string itemName)
        {
            data.DeleteItemInContainer(conName, itemName);
        }

        public static ContentItem GetItemByName(string conName, string itemName)
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

        public static Container GetContainer(string conName)
        {
            return data.GetContainer(conName);
        }

        public static Container GetContainer(int conId)
        {
            return data.GetContainer(conId);
        }

        public static List<ContainerSkinny> GetAllContainers()
        {
            return data.GetAllContainers();
        }

        public static void UpdateContainerName(int conId, string conName)
        {
            data.UpdateContainerName(conId, conName);
        }

        #endregion

    }
}
