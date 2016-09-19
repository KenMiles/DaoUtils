using System;
using DaoUtilsCore.log;
using DaoUtilsCore.log.reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LogManager = DaoUtilsCore.log.LogManager;

namespace TestsDaoUtilsCore.log
{
    [TestClass]
    public class LogManagerTests
    {
        private readonly Mock<ILogManager> _logManager = new Mock<ILogManager>();
        private readonly Mock<ILog> _logger = new Mock<ILog>();

        [TestInitialize]
        public void SetUp()
        {
            // Clear anything already registered/setup - as static
            LogManager.RegisterLogManagerAdaptor(null);
        }

        [TestMethod]
        public void GetLoggerFromType()
        {
            LogManager.RegisterLogManagerAdaptor(_logManager.Object);
            _logManager.Setup(c => c.GetLogger(typeof (LogManagerTests))).Returns(_logger.Object).Verifiable();
            Assert.AreSame(_logger.Object, LogManager.GetLogger(typeof(LogManagerTests)));
            _logManager.Verify();
        }

        [TestMethod]
        public void GetLoggerFromName()
        {
            LogManager.RegisterLogManagerAdaptor(_logManager.Object);
            const string name = "A Log Name";
            _logManager.Setup(c => c.GetLogger(name)).Returns(_logger.Object).Verifiable();
            Assert.AreSame(_logger.Object, LogManager.GetLogger(name));
            _logManager.Verify();
        }

        [TestMethod]
        public void GetLoggerUsingDefault()
        {
            const string name = "A Log Name";
            Assert.IsInstanceOfType(LogManager.GetLogger(name), typeof(LogAdaptor));
            Assert.IsInstanceOfType(LogManager.GetLogger(name), typeof(LogAdaptor));
            // verify not calling mock
            _logManager.Verify();
        }
    }
}
