using System;
using System.Linq;
using System.Reflection;

namespace DaoUtilsCore.log.reflection
{
    class LogManagerFactory
    {
        private static Type GetClassFromName(string className)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Select(c => c.ExportedTypes.FirstOrDefault(t => t.FullName == className))
                .FirstOrDefault(c => c != null);
        }

        private const BindingFlags GetLoggerBindinfFlags = BindingFlags.Static | BindingFlags.Public;
        private static MethodInfo GetLoggerMethod(Type logManagerType, params Type[] nameType)
        {
            return logManagerType?.GetMethod("GetLogger", GetLoggerBindinfFlags, null, nameType, null);
        }

        internal static ILogManager GetLoggerFromClassName(string className)
        {
            try
            {
                var logManagerType = GetClassFromName(className);
                var loggerByType = GetLoggerMethod(logManagerType, typeof(Type));
                var loggerByStr = GetLoggerMethod(logManagerType, typeof(string));
                var loggerType = loggerByStr?.ReturnType;
                if (loggerType == null) return null;
                return new LogManager(loggerByStr, loggerByType, new LogCategoriesReflection(loggerType));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"{e}");
                Console.Write($"{e}");
            }
            return null;
        }

        public static ILogManager GetLogManager()
        {
            return
                GetLoggerFromClassName("log4net.LogManager")
                ?? GetLoggerFromClassName("NLog.LogManager")
                ?? GetLoggerFromClassName("Common.Logging.LogManager");
        }

    }
}