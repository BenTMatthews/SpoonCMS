using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Classes
{
    public class ServiceResponse<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
