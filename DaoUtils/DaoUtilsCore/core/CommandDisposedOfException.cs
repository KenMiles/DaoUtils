using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.core
{
    class CommandDisposedOfException : DaoUtilsException
    {
        public CommandDisposedOfException()
        {
        }

        public CommandDisposedOfException(string message) : base(message)
        {
        }

    }
}
