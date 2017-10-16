﻿using LiteDB;
using SpoonCMS.Classes;
using SpoonCMS.Exceptions;
using SpoonCMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.DataClasses
{
    class LiteDBData : ISpoonData
    {

        public string ConnString { get; set; } = @"Data\DB\SpoonData.db";

        public LiteDBData()
        {

        }

        public LiteDBData(string connString)
        {
            ConnString = connString;
        }

        public Container GetContainer(string conName)
        {
            try
            {
                using (var db = new LiteDatabase(ConnString))
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

        public List<ContainerSkinny> GetAllContainers()
        {
            List<ContainerSkinny> retList = new List<ContainerSkinny>();
            try
            {
                using (var db = new LiteDatabase(ConnString))
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
                using (var db = new LiteDatabase(ConnString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    var existingCon = containers.FindOne(x => x.Name.Equals(container.Name));

                    if (existingCon == null)
                    {
                        containers.Insert(container);
                    }
                    else
                        throw new NameExistsException("A collection with that name already exists");

                    containers.EnsureIndex(x => x.Name);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteContainer(string conName)
        {
            try
            {
                using (var db = new LiteDatabase(ConnString))
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

        public void AddItemToContainer(string conName, IItem item)
        {
            try
            {

                using (var db = new LiteDatabase(ConnString))
                {
                    var containers = db.GetCollection<Container>("Containers");
                    Container container = containers.FindOne(x => x.Name.Equals(conName));

                    if (container != null)
                    {
                        if (!container.Items.TryAdd(item.Name, item))
                        {
                            throw new NameExistsException("An item of that name already exists in this container");
                        }

                        containers.Update(container);
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

        public void UpdateItemInContainer(string conName, IItem item)
        {
            try
            {

                using (var db = new LiteDatabase(ConnString))
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

                using (var db = new LiteDatabase(ConnString))
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

    }
}