using SpoonCMSCore.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Interfaces
{
    public interface ISpoonData
    {
        Container GetContainer(string conName);
        Container GetContainer(int conId);
        List<ContainerSkinny> GetAllContainers();
        void AddContainer(Container container);
        void DeleteContainer(string conName);
        void AddItemToContainer(string conName, ContentItem item);
        void UpdateItemInContainer(string conName, ContentItem item);
        void DeleteItemInContainer(string conName, string itemName);
        void UpdateContainer(Container con);
        void UpdateContainerName(int conId, string conName);
    }
}
