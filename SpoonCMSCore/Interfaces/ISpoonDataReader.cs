using SpoonCMSCore.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Interfaces
{
    public interface ISpoonDataReader
    {
        Container GetContainer(string conName);
        Container GetContainer(int conId);
        List<ContainerSkinny> GetAllContainers();
    }
}
