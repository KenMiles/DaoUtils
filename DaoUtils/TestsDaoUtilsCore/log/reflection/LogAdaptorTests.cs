using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using DaoUtilsCore.log.reflection;
using Moq;

namespace TestsDaoUtilsCore.log.reflection
{
    [TestClass]
    public class LogAdaptorTests
    {
        private readonly Mock<ILoggerCategories> _loggerCategories = new Mock<ILoggerCategories>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategory> _debug = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategory> _info = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategory> _warn = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategory> _error = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategory> _fatal = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private LogAdaptor _uut;
        private readonly object _logger = "A Logger";

        [TestInitialize]
        public void Setup()
        {
            _loggerCategories.Setup(c => c.Error).Returns(_error.Object);
            _loggerCategories.Setup(c => c.Info).Returns(_info.Object);
            _loggerCategories.Setup(c => c.Warn).Returns(_warn.Object);
            _loggerCategories.Setup(c => c.Debug).Returns(_debug.Object);
            _loggerCategories.Setup(c => c.Fatal).Returns(_fatal.Object);
            _uut = new LogAdaptor(_logger, _loggerCategories.Object);
        }

        [TestMethod]
        public void TestDebugEnabled()
        {
            _debug.Setup(c => c.IsEnabled(_logger)).Returns(true).Verifiable();
            Assert.IsTrue(_uut.IsDebugEnabled);
            _debug.Verify();
            _debug.Setup(c => c.IsEnabled(_logger)).Returns(false).Verifiable();
            Assert.IsFalse(_uut.IsDebugEnabled);
            _debug.Verify();
        }

        [TestMethod]
        public void TestDebugMessageAndException()
        {
            var exception = new NullReferenceException();
            var message = "A Message";
            _debug.Setup(c => c.Log(_logger, message, exception)).Verifiable();
            _uut.Debug(message, exception);
            _debug.Verify();
        }

        [TestMethod]
        public void TestDebugMessage()
        {
            var message = "A Different Message";
            _debug.Setup(c => c.Log(_logger, message)).Verifiable();
            _uut.Debug(message);
            _debug.Verify();
        }

        [TestMethod]
        public void TestDebugMessageFormat()
        {
            var fmt = "Another Message";
            var args = new object[] {"One", 2, 3.0}; 
            _debug.Setup(c => c.LogFormat(_logger, fmt, args)).Verifiable();
            _uut.DebugFormat(fmt, args);
            _debug.Verify();
        }

        [TestMethod]
        public void TestDebugMessageFormatAndProvider()
        {
            var fmt = "Message Again";
            var provider = new Mock<IFormatProvider>(MockBehavior.Strict);
            var args = new object[] { "Three", 2, 1.0 };
            _debug.Setup(c => c.LogFormat(_logger, provider.Object, fmt, args)).Verifiable();
            _uut.DebugFormat(provider.Object, fmt, args);
            _debug.Verify();
        }
        [TestMethod]
        public void TestFatalEnabled()
        {
            _fatal.Setup(c => c.IsEnabled(_logger)).Returns(true).Verifiable();
            Assert.IsTrue(_uut.IsFatalEnabled);
            _fatal.Verify();
            _fatal.Setup(c => c.IsEnabled(_logger)).Returns(false).Verifiable();
            Assert.IsFalse(_uut.IsFatalEnabled);
            _fatal.Verify();
        }

        [TestMethod]
        public void TestFatalMessageAndException()
        {
            var exception = new NullReferenceException();
            var message = "A Message";
            _fatal.Setup(c => c.Log(_logger, message, exception)).Verifiable();
            _uut.Fatal(message, exception);
            _fatal.Verify();
        }

        [TestMethod]
        public void TestFatalMessage()
        {
            var message = "A Different Message";
            _fatal.Setup(c => c.Log(_logger, message)).Verifiable();
            _uut.Fatal(message);
            _fatal.Verify();
        }

        [TestMethod]
        public void TestFatalMessageFormat()
        {
            var fmt = "Another Message";
            var args = new object[] { "One", 2, 3.0 };
            _fatal.Setup(c => c.LogFormat(_logger, fmt, args)).Verifiable();
            _uut.FatalFormat(fmt, args);
            _fatal.Verify();
        }

        [TestMethod]
        public void TestFatalMessageFormatAndProvider()
        {
            var fmt = "Message Again";
            var provider = new Mock<IFormatProvider>(MockBehavior.Strict);
            var args = new object[] { "Three", 2, 1.0 };
            _fatal.Setup(c => c.LogFormat(_logger, provider.Object, fmt, args)).Verifiable();
            _uut.FatalFormat(provider.Object, fmt, args);
            _fatal.Verify();
        }

        [TestMethod]
        public void TestErrorEnabled()
        {
            _error.Setup(c => c.IsEnabled(_logger)).Returns(true).Verifiable();
            Assert.IsTrue(_uut.IsErrorEnabled);
            _error.Verify();
            _error.Setup(c => c.IsEnabled(_logger)).Returns(false).Verifiable();
            Assert.IsFalse(_uut.IsErrorEnabled);
            _error.Verify();
        }

        [TestMethod]
        public void TestErrorMessageAndException()
        {
            var exception = new NullReferenceException();
            var message = "A Message";
            _error.Setup(c => c.Log(_logger, message, exception)).Verifiable();
            _uut.Error(message, exception);
            _error.Verify();
        }

        [TestMethod]
        public void TestErrorMessage()
        {
            var message = "A Different Message";
            _error.Setup(c => c.Log(_logger, message)).Verifiable();
            _uut.Error(message);
            _error.Verify();
        }

        [TestMethod]
        public void TestErrorMessageFormat()
        {
            var fmt = "Another Message";
            var args = new object[] { "One", 2, 3.0 };
            _error.Setup(c => c.LogFormat(_logger, fmt, args)).Verifiable();
            _uut.ErrorFormat(fmt, args);
            _error.Verify();
        }

        [TestMethod]
        public void TestErrorMessageFormatAndProvider()
        {
            var fmt = "Message Again";
            var provider = new Mock<IFormatProvider>(MockBehavior.Strict);
            var args = new object[] { "Three", 2, 1.0 };
            _error.Setup(c => c.LogFormat(_logger, provider.Object, fmt, args)).Verifiable();
            _uut.ErrorFormat(provider.Object, fmt, args);
            _error.Verify();
        }

        [TestMethod]
        public void TestWarnEnabled()
        {
            _warn.Setup(c => c.IsEnabled(_logger)).Returns(true).Verifiable();
            Assert.IsTrue(_uut.IsWarnEnabled);
            _warn.Verify();
            _warn.Setup(c => c.IsEnabled(_logger)).Returns(false).Verifiable();
            Assert.IsFalse(_uut.IsWarnEnabled);
            _warn.Verify();
        }

        [TestMethod]
        public void TestWarnMessageAndException()
        {
            var exception = new NullReferenceException();
            var message = "A Message";
            _warn.Setup(c => c.Log(_logger, message, exception)).Verifiable();
            _uut.Warn(message, exception);
            _warn.Verify();
        }

        [TestMethod]
        public void TestWarnMessage()
        {
            var message = "A Different Message";
            _warn.Setup(c => c.Log(_logger, message)).Verifiable();
            _uut.Warn(message);
            _warn.Verify();
        }

        [TestMethod]
        public void TestWarnMessageFormat()
        {
            var fmt = "Another Message";
            var args = new object[] { "One", 2, 3.0 };
            _warn.Setup(c => c.LogFormat(_logger, fmt, args)).Verifiable();
            _uut.WarnFormat(fmt, args);
            _warn.Verify();
        }

        [TestMethod]
        public void TestWarnMessageFormatAndProvider()
        {
            var fmt = "Message Again";
            var provider = new Mock<IFormatProvider>(MockBehavior.Strict);
            var args = new object[] { "Three", 2, 1.0 };
            _warn.Setup(c => c.LogFormat(_logger, provider.Object, fmt, args)).Verifiable();
            _uut.WarnFormat(provider.Object, fmt, args);
            _warn.Verify();
        }

        [TestMethod]
        public void TestInfoEnabled()
        {
            _info.Setup(c => c.IsEnabled(_logger)).Returns(true).Verifiable();
            Assert.IsTrue(_uut.IsInfoEnabled);
            _info.Verify();
            _info.Setup(c => c.IsEnabled(_logger)).Returns(false).Verifiable();
            Assert.IsFalse(_uut.IsInfoEnabled);
            _info.Verify();
        }

        [TestMethod]
        public void TestInfoMessageAndException()
        {
            var exception = new NullReferenceException();
            var message = "A Message";
            _info.Setup(c => c.Log(_logger, message, exception)).Verifiable();
            _uut.Info(message, exception);
            _info.Verify();
        }

        [TestMethod]
        public void TestInfoMessage()
        {
            var message = "A Different Message";
            _info.Setup(c => c.Log(_logger, message)).Verifiable();
            _uut.Info(message);
            _info.Verify();
        }

        [TestMethod]
        public void TestInfoMessageFormat()
        {
            var fmt = "Another Message";
            var args = new object[] { "One", 2, 3.0 };
            _info.Setup(c => c.LogFormat(_logger, fmt, args)).Verifiable();
            _uut.InfoFormat(fmt, args);
            _info.Verify();
        }

        [TestMethod]
        public void TestInfoMessageFormatAndProvider()
        {
            var fmt = "Message Again";
            var provider = new Mock<IFormatProvider>(MockBehavior.Strict);
            var args = new object[] { "Three", 2, 1.0 };
            _info.Setup(c => c.LogFormat(_logger, provider.Object, fmt, args)).Verifiable();
            _uut.InfoFormat(provider.Object, fmt, args);
            _info.Verify();
        }
    }
}