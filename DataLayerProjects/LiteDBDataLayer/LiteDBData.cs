using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using SpoonCMSCore.Classes;
using SpoonCMSCore.Exceptions;
using SpoonCMSCore.Interfaces;
using SpoonCMSCore.Utilities;

namespace SpoonCMSCore.LiteDBDatalayer
{
    public class LiteDBData : LiteDBDataReader, ISpoonData
    {

        public LiteDBData() : base()
        {            
            
        }

        /// <summary>
        /// A path to the directory to store the DB
        /// </summary>
        /// <param name="connString"></param>
        public LiteDBData(string connString) : base(connString)
        {
             
        }

        public void AddContainer(Container container)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    var existingCon = containers.FindOne(x => x.Name.Equals(container.Name));

                    if (existingCon == null)
                    {
                        containers.Insert(container);
                    }
                    else
                        throw new NameExistsException("A container with that name already exists");

                    containers.EnsureIndex(x => x.Name);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void UpdateContainer(Container container)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    var existingCon = containers.FindOne(x => x.Id.Equals(container.Id));

                    if (existingCon == null)
                    {
                        throw new ContainerDoesNotExistException("Container with that Id does not exist");
                    }
                    else
                    {

                        Utils.UpdateContainer(existingCon, container);

                        containers.Update(existingCon);
                        containers.EnsureIndex(x => x.Name);
                    }                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteContainer(string conName)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    containers.Delete((Query.EQ("Name", conName)));
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void AddItemToContainer(string conName, ContentItem item)
        {
            try
            {

                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container container = containers.FindOne(x => x.Name.Equals(conName));

                    if (container != null)
                    {
                        if(!container.Items.ContainsKey(item.Name))
                        {
                            container.Items.Add(item.Name, item);
                            containers.Update(container);
                        }    
                        else
                        {
                            throw new NameExistsException("An item with that name already exists");
                        }
                    }
                    else
                    {
                        throw new ContainerDoesNotExistException("The container name is not in the database collection");
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void UpdateItemInContainer(string conName, ContentItem item)
        {
            try
            {

                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container container = containers.FindOne(x => x.Name.Equals(conName));

                    if (container != null)
                    {
                        if (container.Items.ContainsKey(item.Name))
                        {
                            container.Items[item.Name] = Utils.UpdateContentItem(container.Items[item.Name], item);                        

                            containers.Update(container);
                        }
                        else
                        {
                            throw new ItemDoesNotExistException("The Item with the specified name does not exist.");
                        }
                    }
                    else
                    {
                        throw new ContainerDoesNotExistException("The container name is not in the database collection");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteItemInContainer(string conName, string itemName)
        {
            try
            {

                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container container = containers.FindOne(x => x.Name.Equals(conName));

                    if (container != null)
                    {
                        container.Items.Remove(itemName);

                        containers.Update(container);
                    }
                    else
                    {
                        throw new ContainerDoesNotExistException("The container name is not in the database collection");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateContainerName(int conId, string conName)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container container = containers.FindOne(x => x.Id.Equals(conId));

                    if (container != null)
                    {
                        if (container.Name != conName) // Don't do anything, it's the same name we already have
                        {
                            var existingCon = containers.FindOne(x => x.Name.Equals(conName)); //Make sure name isn't already taken
                            if (existingCon == null)
                            {
                                container.Name = conName;
                                containers.Update(container);
                            }
                            else
                            {
                                throw new NameExistsException("A container with that name already exists");
                            }
                        }
                    }
                    else
                    {
                        throw new ContainerDoesNotExistException("The container Id is not in the database collection");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
