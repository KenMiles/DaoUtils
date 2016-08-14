using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.core
{
    public class DaoUtilsException : Exception
    {
        public DaoUtilsException()
        {
        }

        public DaoUtilsException(string message) : base(message)
        {
        }

        public DaoUtilsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DaoUtilsException(Exception innerException) : base(innerException.Message, innerException)
        {
        }
    }
    
}
