using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DaoUtilsCore.log
{
    public class LogManager
    {
        private static ILogManager _logManager;
        public static void RegisterLogManagerAdaptor(ILogManager logManager)
        {
            _logManager = logManager;
        }

        private static ILogManager ActiveLogManager()
        {
            return _logManager = _logManager ?? reflection.LogManagerFactory.GetLogManager() ?? console.LogManager.Instance;
        }

        public static ILog GetLogger(string name)
        {
            return ActiveLogManager().GetLogger(name);
        }

        public static ILog GetLogger(Type type)
        {
            return ActiveLogManager().GetLogger(type);
        }
    }
}
