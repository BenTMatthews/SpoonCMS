using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMSCore.Exceptions
{
    public class ContainerDoesNotExistException : Exception
    {
        public ContainerDoesNotExistException()
        {
        }

        public ContainerDoesNotExistException(string message)
            : base(message)
        {
        }

        public ContainerDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
