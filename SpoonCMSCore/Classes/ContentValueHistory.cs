using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Classes
{
    public class ContentValueHistory
    {
        public string Value { get; set; }
        public DateTime Changed { get; set; }
        public string ValueDiffHTML { get; set; }
    }
}
