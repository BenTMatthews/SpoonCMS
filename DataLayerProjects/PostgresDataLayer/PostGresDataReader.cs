using SpoonCMSCore.Classes;
using SpoonCMSCore.Exceptions;
using SpoonCMSCore.Interfaces;
using System;
using System.Collections.Generic;

using Marten;
using Marten.Services;
using System.Linq;
using Weasel.Postgresql;
using Weasel.Core;

namespace SpoonCMSCore.PostGresData
{
    public class PostGresDataReader : ISpoonDataReader
    {
        protected DocumentStore _store { get; }
        protected string _connString { get; set; }

        public PostGresDataReader(string connString)
        {
            _connString = connString;

            _store = DocumentStore.For(x =>
            {
                x.Connection(_connString);
                x.Serializer(new JsonNetSerializer { EnumStorage = EnumStorage.AsInteger });

                x.Schema.For<Container>().Duplicate(y => y.Id, configure: y => y.IsUnique = true);

                x.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            });
        }

        public Container GetContainer(string conName)
        {
            try
            {
                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Name == conName)
                                            .FirstOrDefault();

                    return container;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Container GetContainer(int conId)
        {
            try
            {
                using (var session = _store.LightweightSession())
                {
                    Container container = session.Query<Container>()
                                            .Where(x => x.Id == conId)
                                            .FirstOrDefault();

                    return container;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ContainerSkinny> GetAllContainers() //Need to make this lighterweight, this could get heavy
        {
            List<ContainerSkinny> retList = new List<ContainerSkinny>();
            try
            {
                using (var session = _store.LightweightSession())
                {
                    var containers = session.Query<Container>();

                    foreach (Container con in containers)
                    {
                        retList.Add(con.GetSkinny());
                    }
                }

                return retList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
