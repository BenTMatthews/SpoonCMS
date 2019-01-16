using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Interfaces
{
    public interface IItem
    {
        Guid Id { get; set; }
        object Value { get; set; }
        bool Active { get; set; }
        DateTime BeginDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime Created { get; }
        String Name { get; set; }         
    }
}
