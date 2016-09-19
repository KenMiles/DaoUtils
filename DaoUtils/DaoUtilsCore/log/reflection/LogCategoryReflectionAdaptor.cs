using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DaoUtilsCore.log.reflection
{
    class LogCategoryReflectionAdaptor : ILoggerCategory
    {
        private readonly MethodInfo _logMessage;
        private readonly MethodInfo _logMessageAndException;
        private readonly MethodInfo _logExceptionMessageAndArgs;
        private readonly MethodInfo _logFormat;
        private readonly MethodInfo _logFormatWithProvider;
        private readonly PropertyInfo _isEnabled;

        public LogCategoryReflectionAdaptor(string categoryName, Type loggerType)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentNullException(nameof(categoryName));
            if (loggerType == null) throw new ArgumentNullException(nameof(loggerType));
            const BindingFlags bfs = BindingFlags.Instance | BindingFlags.Public;
            //NLog 4 moved to exception, message, args - so if we see this use it in pref to message, exception
            _logExceptionMessageAndArgs = loggerType.GetMethod(categoryName, bfs, null, new Type[] { typeof(Exception), typeof(string), typeof(object[]) }, null);
            _logMessageAndException = loggerType.GetMethod(categoryName, bfs, null, new Type[] { typeof(string), typeof(Exception) }, null);
            if (_logMessageAndException == null && _logExceptionMessageAndArgs == null)
            {
                throw new Exception($"Unable to find method {categoryName}(string, Exception)");
            }
            _logMessage = loggerType.GetMethod(categoryName, bfs, null, new Type[] { typeof(string) }, null);
            _logFormat = loggerType.GetMethod($"{categoryName}Format", bfs, null, new Type[] { typeof(string), typeof(object[]) }, null);
            _logFormatWithProvider = loggerType.GetMethod($"{categoryName}Format", bfs, null, new Type[] { typeof(IFormatProvider), typeof(string), typeof(object[]) }, null);
            _isEnabled = loggerType.GetProperty($"Is{categoryName}Enabled");
        }

        public bool IsEnabled(object logger)
        {
            // note we want anything we don't understand to return true
            return !false.Equals(_isEnabled?.GetValue(logger));
        }

        public void Log(object logger, object message)
        {
            if (_logMessage == null) Log(logger, message, null);
            _logMessage?.Invoke(logger, new[] { message });
        }

        public void Log(object logger, object message, Exception exception)
        {
            if (_logExceptionMessageAndArgs != null)
            {
                _logExceptionMessageAndArgs.Invoke(logger, new[] { exception, message, new object[0]});
            }
            else
            {
                _logMessageAndException.Invoke(logger, new[] { message, exception });
            }
        }

        public void LogFormat(object logger, string format, params object[] args)
        {
            if (_logFormat == null) Log(logger, string.Format(format, args));
            _logFormat?.Invoke(logger, new object[] { format, args });
        }

        public void LogFormat(object logger, IFormatProvider provider, string format, params object[] args)
        {
            if (_logFormatWithProvider == null) Log(logger, string.Format(provider, format, args));
            _logFormatWithProvider?.Invoke(logger, new object[] { provider, format, args });
        }
    }
}
