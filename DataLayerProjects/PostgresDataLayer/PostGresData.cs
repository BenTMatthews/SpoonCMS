using SpoonCMSCore.Classes;
using SpoonCMSCore.Exceptions;
using SpoonCMSCore.Interfaces;
using System;
using System.Collections.Generic;

using Marten;
using Marten.Services;
using System.Linq;

namespace SpoonCMSCore.PostGresData
{
    public class PostGresData : PostGresDataReader, ISpoonData
    {
        public PostGresData(string connString): base(connString)
        {
            
        }

        public void AddContainer(Container container)
        {
            try
            {
                using (var session = _store.LightweightSession())
                {
                    bool isFound = session.Query<Container>()
                                        .Where(x => x.Name == container.Name)
                                        .Any();

                    if (!isFound)
                    {
                        session.Store<Container>(container);
                        session.SaveChanges();
                    }
                    else
                    {
                        throw new NameExistsException("A container with that name already exists");
                    }
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
                using (var session = _store.LightweightSession())
                {
                    var existingCon = session.Query<Container>()
                                            .Where(x => x.Id == container.Id)
                                            .FirstOrDefault();


                    if (existingCon == null)
                    {
                        throw new ContainerDoesNotExistException("Container with that Id does not exist");
                    }
                    else
                    {
                        existingCon.Name = container.Name;
                        existingCon.Active = container.Active;
                        existingCon.Items = container.Items;

                        session.Update<Container>(existingCon);
                        session.SaveChanges();
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
                using(var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Name == conName)
                                            .FirstOrDefault();

                    if (container != null)
                    {
                        session.Delete(container);
                        session.SaveChanges();
                    }
                    else
                    {
                        throw new ItemDoesNotExistException("Container not found");
                    }
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

                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Name == conName)
                                            .FirstOrDefault();

                    if (container != null)
                    {
                        if(!container.Items.ContainsKey(item.Name))
                        {
                            container.Items.Add(item.Name, item);
                            session.Update<Container>(container);
                            session.SaveChanges();
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

                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Name == conName)
                                            .FirstOrDefault();

                    if (container != null)
                    {
                        if (container.Items.ContainsKey(item.Name))
                        {
                            container.Items[item.Name].Active = item.Active;
                            container.Items[item.Name].BeginDate = item.BeginDate;
                            container.Items[item.Name].EndDate = item.EndDate;
                            container.Items[item.Name].Value = item.Value;

                            //Why was I not doing this before? Did I miss it or is there a reason?
                            container.Items[item.Name].Priority = item.Priority;

                            session.Update<Container>(container);
                            session.SaveChanges();
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

                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Name == conName)
                                            .FirstOrDefault();

                    if (container != null)
                    {
                        container.Items.Remove(itemName);

                        session.Update<Container>(container);
                        session.SaveChanges();
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
                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Id == conId)
                                            .FirstOrDefault();

                    if (container != null)
                    {
                        if (container.Name != conName) // Don't do anything, it's the same name we already have
                        {
                            var existingCon = session.Query<Container>()   //Make sure name isn't already taken
                                                .Where(x => x.Name == conName)
                                                .FirstOrDefault(); 

                            if (existingCon == null)
                            {
                                container.Name = conName;
                                session.Update<Container>(container);
                                session.SaveChanges();
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
