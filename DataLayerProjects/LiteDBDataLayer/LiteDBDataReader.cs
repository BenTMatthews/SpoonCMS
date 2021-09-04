using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using SpoonCMSCore.Classes;
using SpoonCMSCore.Exceptions;
using SpoonCMSCore.Interfaces;

namespace SpoonCMSCore.LiteDBDatalayer
{
    public class LiteDBDataReader : ISpoonDataReader
    {

        protected string _connString { get; set; }
        protected string _defaultDir { get; set; } = @"Data\DB\";
        protected readonly string _dbFileName = "SpoonData.db";

        public LiteDBDataReader()
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
        public LiteDBDataReader(string connString)
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
