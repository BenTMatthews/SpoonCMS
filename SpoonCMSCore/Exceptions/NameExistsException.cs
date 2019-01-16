using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Exceptions
{
    public class NameExistsException : Exception
    {
        public NameExistsException()
        {
        }

        public NameExistsException(string message)
            : base(message)
        {
        }

        public NameExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
