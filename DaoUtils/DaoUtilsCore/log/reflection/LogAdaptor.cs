using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DaoUtilsCore.log.reflection
{

    class LogAdaptor: ILog
    {
        private readonly object _logger;
        private readonly ILoggerCategories _categorys;

        public LogAdaptor(object logger, ILoggerCategories categorys)
        {
            _logger = logger;
            _categorys = categorys;
        }

        public bool IsDebugEnabled => _categorys.Debug.IsEnabled(_logger);

        public void Debug(object message)
        {
            _categorys.Debug.Log(_logger, message);
        }

        public void Debug(object message, Exception exception)
        {
            _categorys.Debug.Log(_logger, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _categorys.Debug.LogFormat(_logger, format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _categorys.Debug.LogFormat(_logger, provider, format, args);
        }

        public bool IsInfoEnabled => _categorys.Info.IsEnabled(_logger);

        public void Info(object message)
        {
            _categorys.Info.Log(_logger, message);
        }

        public void Info(object message, Exception exception)
        {
            _categorys.Info.Log(_logger, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _categorys.Info.LogFormat(_logger, format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _categorys.Info.LogFormat(_logger, provider, format, args);
        }

        public bool IsWarnEnabled => _categorys.Warn.IsEnabled(_logger);

        public void Warn(object message)
        {
            _categorys.Warn.Log(_logger, message);
        }

        public void Warn(object message, Exception exception)
        {
            _categorys.Warn.Log(_logger, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _categorys.Warn.LogFormat(_logger, format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _categorys.Warn.LogFormat(_logger, provider, format, args);
        }

        public bool IsErrorEnabled => _categorys.Error.IsEnabled(_logger);

        public void Error(object message)
        {
            _categorys.Error.Log(_logger, message);
        }

        public void Error(object message, Exception exception)
        {
            _categorys.Error.Log(_logger, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _categorys.Error.LogFormat(_logger, format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _categorys.Error.LogFormat(_logger, provider, format, args);
        }

        public bool IsFatalEnabled => _categorys.Fatal.IsEnabled(_logger);

        public void Fatal(object message)
        {
            _categorys.Fatal.Log(_logger, message);
        }

        public void Fatal(object message, Exception exception)
        {
            _categorys.Fatal.Log(_logger, message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _categorys.Fatal.LogFormat(_logger, format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _categorys.Fatal.LogFormat(_logger, provider, format, args);
        }

    }
}
