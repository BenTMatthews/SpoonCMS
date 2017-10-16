using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.DataClasses
{
    public class DBSettings
    {
        public string Type { get; set; } = "LiteDB";
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
