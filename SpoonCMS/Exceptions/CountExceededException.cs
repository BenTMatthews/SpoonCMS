using System;
using System.Collections.Generic;
using System.Text;

namespace SpoonCMS.Exceptions
{
    public class CountExceededException : Exception
    {
        public CountExceededException()
        {
        }

        public CountExceededException(string message)
            : base(message)
        {
        }

        public CountExceededException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
