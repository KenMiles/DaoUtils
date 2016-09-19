using System;
using System.Collections.Generic;
using System.Text;
using DaoUtilsCore.log.reflection;

namespace DaoUtilsCore.log.console
{
    class LogCatgeoriesConsole: LogCategories
    {
        public LogCatgeoriesConsole() : base(
            new LogCategoryConsole("Debug", false, false),
            new LogCategoryConsole("Info", false, false),
            new LogCategoryConsole("Warn", false, false),
            new LogCategoryConsole("Error", false,true),
            new LogCategoryConsole("Fatal", false,true)
            )
        {
        }
    }
}
