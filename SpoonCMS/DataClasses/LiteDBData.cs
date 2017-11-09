using LiteDB;
using SpoonCMS.Classes;
using SpoonCMS.Exceptions;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpoonCMS.DataClasses
{
    class LiteDBData : ISpoonData
    {

        private string _connString { get; set; }
        private string _defaultDir { get; set; } = @"Data\DB\";
        private string _dbFileName = "SpoonData.db";

        public LiteDBData()
        {            
            if (!Directory.Exists(_defaultDir))
            {
                Directory.CreateDirectory(_defaultDir);
            }
            _connString = _defaultDir + _dbFileName;
        }

        /// <summary>
        /// A path to the directory to store the DB
        /// </summary>
        /// <param name="connString"></param>
        public LiteDBData(string connString)
        {
            if (!string.IsNullOrEmpty(connString))
            {
                if (!Directory.Exists(connString))
                {
                    Directory.CreateDirectory(connString);
                }
                _connString = connString + _dbFileName;
            }
            else
            {
                if (!Directory.Exists(_defaultDir))
                {
                    Directory.CreateDirectory(_defaultDir);
                }
                _connString = _defaultDir + _dbFileName;
            }            
        }

        public Container GetContainer(string conName)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container existingCon = containers.FindOne(x => x.Name.Equals(conName));

                    return existingCon;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Container GetContainer(int conId)
        {
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container existingCon = containers.FindOne(x => x.Id.Equals(conId));

                    return existingCon;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ContainerSkinny> GetAllContainers()
        {
            List<ContainerSkinny> retList = new List<ContainerSkinny>();
            try
            {
                using (var db = new LiteDatabase(_connString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    
                    foreach(Container con in containers.FindAll())
                    {
                        retList.Add(con.GetSkinny());
                    }
                }

                return retList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            catch(Exception ex)
            {
                throw ex;
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
                        existingCon.Name = container.Name;
                        existingCon.Active = container.Active;
                        existingCon.Items = container.Items;

                        containers.Update(container);
                        containers.EnsureIndex(x => x.Name);
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            catch(Exception ex)
            {
                throw ex;
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
            catch(Exception ex)
            {
                throw ex;
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
                            container.Items[item.Name].Active = item.Active;
                            container.Items[item.Name].BeginDate = item.BeginDate;
                            container.Items[item.Name].EndDate = item.EndDate;
                            container.Items[item.Name].Value = item.Value;

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
            catch (Exception ex)
            {
                throw ex;
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
            catch (Exception ex)
            {
                throw ex;
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
                        container.Name = conName;
                        containers.Update(container);
                    }
                    else
                    {
                        throw new ContainerDoesNotExistException("The container Id is not in the database collection");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
