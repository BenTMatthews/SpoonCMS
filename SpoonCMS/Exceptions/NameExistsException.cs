using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Exceptions
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
