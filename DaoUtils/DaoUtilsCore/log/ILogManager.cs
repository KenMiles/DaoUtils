using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.log
{
    public interface ILogManager
    {
        ILog GetLogger(string name);
        ILog GetLogger(Type type);
    }
}
