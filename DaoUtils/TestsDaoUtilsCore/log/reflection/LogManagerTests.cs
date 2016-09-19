using System;
using System.Collections.Generic;
using System.Reflection;
using DaoUtilsCore.log;
using DaoUtilsCore.log.reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LogManager = DaoUtilsCore.log.reflection.LogManager;

namespace TestsDaoUtilsCore.log.reflection
{
    [TestClass]
    public class LogManagerTests
    {
        private static List<Tuple<string, Type, Mock<ILog>>> _createdLogs = new List<Tuple<string, Type, Mock<ILog>>>();
        public static ILog GetLoggerByNameForTests(string name)
        {
            var result = new Mock<ILog>(MockBehavior.Strict);
            _createdLogs.Add(new Tuple<string, Type, Mock<ILog>>(name, null, result));
            return result.Object;
        }

        public static ILog GetLoggerByTypeForTests(Type type)
        {
            var result = new Mock<ILog>(MockBehavior.Strict);
            _createdLogs.Add(new Tuple<string, Type, Mock<ILog>>(null, type, result));
            return result.Object;
        }

        private MethodInfo _getLoggerFromStr;
        private MethodInfo _getLoggerFromType;
        private readonly Mock<ILoggerCategory> _error = new Mock<ILoggerCategory>(MockBehavior.Strict);
        private readonly Mock<ILoggerCategories> _categories = new Mock<ILoggerCategories>(MockBehavior.Strict);

        [TestInitialize]
        public void SetUp()
        {
            _getLoggerFromStr = GetType().GetMethod("GetLoggerByNameForTests");
            _getLoggerFromType = GetType().GetMethod("GetLoggerByTypeForTests");
            _categories.Setup(c => c.Error).Returns(_error.Object);
            _createdLogs.Clear();
        }

        [TestCleanup]
        public void ClearDown()
        {
            _createdLogs.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoesntLikeNullCategories()
        {
            var result = new LogManager(_getLoggerFromStr, _getLoggerFromType, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoesntLikeNullLoggerFromStr()
        {
            var result = new LogManager(null, _getLoggerFromType, _categories.Object);
        }

        private void CheckLogger(ILog logger, string expectedNamePassed = null, Type expectedTypePassed = null )
        {
            Assert.IsInstanceOfType(logger, typeof(LogAdaptor));
            Assert.AreEqual(1, _createdLogs.Count);
            var created = _createdLogs[0];
            if (expectedNamePassed != null)
            {
                Assert.AreEqual(expectedNamePassed, created.Item1);
                Assert.IsNull(created.Item2);
            }
            if (expectedTypePassed != null)
            {
                Assert.AreEqual(expectedTypePassed, created.Item2);
                Assert.IsNull(created.Item1);
            }
            // Now quick check to see categories passed in
            _error.Setup(c => c.Log(_createdLogs[0].Item3.Object, "A Test")).Verifiable();
            logger.Error("A Test");
            _error.Verify();
        }

        [TestMethod]
        public void LoggerFromStr()
        {
            var logManager = new LogManager(_getLoggerFromStr, _getLoggerFromType, _categories.Object);
            CheckLogger(logManager.GetLogger("A String"), "A String");
        }

        [TestMethod]
        public void LoggerFromType()
        {
            var logManager = new LogManager(_getLoggerFromStr, _getLoggerFromType, _categories.Object);
            CheckLogger(logManager.GetLogger(GetType()), null, GetType());
        }

        [TestMethod]
        public void LoggerFromTypeFallsBackToStrIfNoByType()
        {
            var logManager = new LogManager(_getLoggerFromStr, null, _categories.Object);
            CheckLogger(logManager.GetLogger(GetType()), GetType().FullName);
        }
    }
}
