using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.DataClasses
{
    //For future proofing and config when we have other data providers.
    public class DBSettings
    {
        public string Type { get; set; } = "LiteDB";
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
