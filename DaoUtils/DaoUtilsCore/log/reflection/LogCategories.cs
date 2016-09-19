using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.log.reflection
{
    class LogCategories: ILoggerCategories
    {
        public LogCategories(
            ILoggerCategory debug, 
            ILoggerCategory info, 
            ILoggerCategory warn, 
            ILoggerCategory error, 
            ILoggerCategory fatal)
        {
            Debug = debug;
            Info = info;
            Error = error;
            Warn = warn;
            Fatal = fatal;
        }

        public ILoggerCategory Debug { get; }
        public ILoggerCategory Info { get; }
        public ILoggerCategory Warn { get; }
        public ILoggerCategory Error { get; }
        public ILoggerCategory Fatal { get; }
    }

    internal class LogCategoriesReflection: LogCategories
    {
        public LogCategoriesReflection(Type logger) : base(
            new LogCategoryReflectionAdaptor("Debug", logger),
            new LogCategoryReflectionAdaptor("Info", logger),
            new LogCategoryReflectionAdaptor("Warn", logger),
            new LogCategoryReflectionAdaptor("Error", logger),
            new LogCategoryReflectionAdaptor("Fatal", logger)
            )
        {
        }
    }
}
