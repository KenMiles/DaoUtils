using System;
using System.Collections.Generic;
using System.Text;
using DaoUtilsCore.log.reflection;

namespace DaoUtilsCore.log.console
{
    class LogManager: ILogManager
    {
        private readonly ILoggerCategories _categories = new LogCatgeoriesConsole();
        public ILog GetLogger(string name)
        {
            return new LogAdaptor(name, _categories);
        }

        public ILog GetLogger(Type type)
        {
            return new LogAdaptor(type.FullName, _categories);
        }

        public static ILogManager Instance { get; } = new LogManager();
    }
}
