using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Exceptions
{
    public class ItemDoesNotExistException : Exception
    {
        public ItemDoesNotExistException()
        {
        }

        public ItemDoesNotExistException(string message)
            : base(message)
        {
        }

        public ItemDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
