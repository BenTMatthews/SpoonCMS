using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Exceptions
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
