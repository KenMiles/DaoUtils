using System;
using System.Collections.Generic;
using System.Reflection;

namespace DaoUtilsCore.log.reflection
{
    class LogManager: ILogManager
    {

        private readonly MethodInfo _getLoggerFromStr;
        private readonly MethodInfo _getLoggerFromType;
        private readonly ILoggerCategories _categories;

        public LogManager(MethodInfo getLoggerFromStr, MethodInfo getLoggerFromType, ILoggerCategories categories)
        {
            if (getLoggerFromStr == null) throw new ArgumentNullException(nameof(getLoggerFromStr));
            if (categories == null) throw new ArgumentNullException(nameof(categories));
            _getLoggerFromStr = getLoggerFromStr;
            _getLoggerFromType = getLoggerFromType;
            _categories = categories;
        }

        public ILog GetLogger(string name)
        {
            return new LogAdaptor(_getLoggerFromStr.Invoke(null, new object[]{name}), _categories);
        }

        public ILog GetLogger(Type type)
        {
            if (_getLoggerFromType == null) return GetLogger(type?.FullName??"Unknown.type");
            return new LogAdaptor(_getLoggerFromType.Invoke(null, new object[] { type }), _categories);
        }
    }
}
