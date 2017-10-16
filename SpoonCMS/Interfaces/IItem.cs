using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Interfaces
{
    public interface IItem
    {
        int Id { get; set; }
        object Value { get; set; }
        bool Active { get; set; }
        DateTime BeginDate { get; set; }
        DateTime EndDate { get; set; }
        DateTime Created { get; }
        String Name { get; set; }         
    }
}
