using System;
using System.Collections.Generic;
using DaoUtilsCore.log;
using DaoUtilsCore.log.reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.log.reflection
{
    public class TestLogManagerVoidGetLogger
    {
        public static void GetLogger(string name)
        {
        }
    }

    public class TestLogManagerStrOnlyGetLogger
    {
        internal static Mock<ILog> MockedLog = new Mock<ILog>(MockBehavior.Strict);
        internal static List<string> CalledStrs = new List<string>();
        public static ILog GetLogger(string name)
        {
            CalledStrs.Add(name);
            return MockedLog.Object;
        }
    }

    public class TestLogManager
    {
        internal static Mock<ILog> MockedLog = new Mock<ILog>(MockBehavior.Strict);
        internal static List<Type> CalledTypes = new List<Type>();
        internal static List<string> CalledStrs = new List<string>();
        public static ILog GetLogger(Type type)
        {
            CalledTypes.Add(type);
            return MockedLog.Object;
        }

        public static ILog GetLogger(string name)
        {
            CalledStrs.Add(name);
            return MockedLog.Object;
        }
    }

    [TestClass]
    public class LogManagerFactoryTests
    {
        [TestInitialize]
        public void SetUp()
        {
            TestLogManagerStrOnlyGetLogger.CalledStrs.Clear();
            TestLogManager.CalledTypes.Clear();
        }

        [TestMethod]
        public void GetLoggerFromClassNameInvalidType()
        {
            Assert.IsNull(LogManagerFactory.GetLoggerFromClassName("Object"));
            Assert.IsNull(LogManagerFactory.GetLoggerFromClassName(typeof(TestLogManagerVoidGetLogger).FullName));
        }


        [TestMethod]
        public void GetLoggerByType()
        {
            var logFactory = LogManagerFactory.GetLoggerFromClassName(typeof(TestLogManager).FullName);
            Assert.IsNotNull(logFactory);
            var logger = logFactory.GetLogger(GetType());
            Assert.IsInstanceOfType(logger, typeof(LogAdaptor));
            Assert.AreEqual(1, TestLogManager.CalledTypes.Count);
            Assert.AreEqual(0, TestLogManagerStrOnlyGetLogger.CalledStrs.Count);
            Assert.AreEqual(GetType(), TestLogManager.CalledTypes[0]);
            TestLogManager.MockedLog.Setup(c => c.Error("A Test")).Verifiable();
            logger.Error("A Test");
            TestLogManager.MockedLog.Verify();
        }

        [TestMethod]
        public void GetLoggerByName()
        {
            var logFactory = LogManagerFactory.GetLoggerFromClassName(typeof(TestLogManager).FullName);
            Assert.IsNotNull(logFactory);
            var logger = logFactory.GetLogger("A Log Name");
            Assert.IsInstanceOfType(logger, typeof(LogAdaptor));
            Assert.AreEqual(0, TestLogManager.CalledTypes.Count);
            Assert.AreEqual(1, TestLogManager.CalledStrs.Count);
            Assert.AreEqual("A Log Name", TestLogManager.CalledStrs[0]);
            TestLogManager.MockedLog.Setup(c => c.Error("A Test")).Verifiable();
            logger.Error("A Test");
            TestLogManager.MockedLog.Verify();
        }

        [TestMethod]
        public void GetLoggerByTypeFallsToNameIfNoByType()
        {
            var logFactory = LogManagerFactory.GetLoggerFromClassName(typeof(TestLogManagerStrOnlyGetLogger).FullName);
            Assert.IsNotNull(logFactory);
            var logger = logFactory.GetLogger(GetType());
            Assert.IsInstanceOfType(logger, typeof(LogAdaptor));
            Assert.AreEqual(1, TestLogManagerStrOnlyGetLogger.CalledStrs.Count);
            Assert.AreEqual(GetType().FullName, TestLogManagerStrOnlyGetLogger.CalledStrs[0]);
            TestLogManagerStrOnlyGetLogger.MockedLog.Setup(c => c.Error("A Test")).Verifiable();
            logger.Error("A Test");
            TestLogManagerStrOnlyGetLogger.MockedLog.Verify();
        }
    }
}
