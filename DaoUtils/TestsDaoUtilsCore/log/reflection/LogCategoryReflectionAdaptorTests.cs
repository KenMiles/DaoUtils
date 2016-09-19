using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaoUtilsCore.log;
using DaoUtilsCore.log.reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.log.reflection
{
    public class TestLogger
    {
        public List<string> Calls { get; } = new List<string>();

        public void Log4NetStyle(string message, Exception e)
        {
            Calls.Add($"Log4NetStyle(\"{message}\",{e})");
        }

        public void NLogStyle(string message, Exception e)
        {
            Calls.Add($"NLogStyle(\"{message}\",{e})");
        }

        public void NLogStyle(Exception e, string message, object[] args)
        {
            Calls.Add($"NLogStyle({e},\"{message}\", args - {args})");
        }


        public object IsDebugEnabled { get; set; }

        public void Debug(object message)
        {
            Calls.Add($"Debug(\"{message}\")");
        }

        public void Debug(object message, Exception exception)
        {
            Calls.Add($"Debug(\"{message}\", {exception})");
        }

        public void DebugFormat(string format, params object[] args)
        {
            Calls.Add($"Debug(\"{format}\", object[{args.Length}] = {string.Join(", ",args.Select(c => $"{c}"))})");
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Calls.Add($"Debug({provider}, \"{format}\", object[{args.Length}] = {string.Join(", ", args.Select(c => $"{c}"))})");
        }
    }

    [TestClass]
    public class LogCategoryReflectionAdaptorTests
    {
        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DoesntLikeNullLoggerType()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor("Debug", null);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DoesntLikeNullCategory()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor(null, typeof (ILog));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void DoesntLikeEmptyCategory()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor(" ", typeof (ILog));
        }

        [TestMethod]
        public void IfCategoryLoggingMissing()
        {
            try
            {
                var catAdaptor = new LogCategoryReflectionAdaptor("TestCat", typeof (ILog));
                Assert.Fail("Should have raised Exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Unable to find method TestCat(string, Exception)", e.Message);
            }
        }

        [TestMethod]
        public void IfOnlyLogMessageException()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor("Log4NetStyle", typeof (TestLogger));
            var logger = new TestLogger();
            catAdaptor.Log(logger, "A Message");
            catAdaptor.Log(logger, "A Message and Exception", new ArithmeticException("Test Exception"));
            catAdaptor.LogFormat(logger, "A Message and {0}, {1}", "Arguments", "List");
            catAdaptor.LogFormat(logger, CultureInfo.GetCultureInfo("de-DE"), "A Message and {0}, {1}", "Arguments", 1.1);
            Assert.AreEqual(@"Log4NetStyle(""A Message"",)
Log4NetStyle(""A Message and Exception"",System.ArithmeticException: Test Exception)
Log4NetStyle(""A Message and Arguments, List"",)
Log4NetStyle(""A Message and Arguments, 1,1"",)", string.Join("\r\n", logger.Calls));
        }

        [TestMethod]
        public void IfOnlyLogExceptionMessageArgsOverridesMessageException()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor("NLogStyle", typeof (TestLogger));
            var logger = new TestLogger();
            catAdaptor.Log(logger, "A Message");
            catAdaptor.Log(logger, "A Message and Exception", new ArithmeticException("Test Exception"));
            catAdaptor.LogFormat(logger, "A Message and {0}, {1}", "Arguments", "List");
            catAdaptor.LogFormat(logger, CultureInfo.GetCultureInfo("de-DE"), "A Message and {0}, {1}", "Arguments", 1.1);
            Assert.AreEqual(@"NLogStyle(,""A Message"", args - System.Object[])
NLogStyle(System.ArithmeticException: Test Exception,""A Message and Exception"", args - System.Object[])
NLogStyle(,""A Message and Arguments, List"", args - System.Object[])
NLogStyle(,""A Message and Arguments, 1,1"", args - System.Object[])", string.Join("\r\n", logger.Calls));
        }

        [TestMethod]
        public void IsEnabled()
        {
            var logger = new TestLogger();

            var catAdaptorNoIsEnabled = new LogCategoryReflectionAdaptor("Log4NetStyle", typeof(TestLogger));
            Assert.IsTrue(catAdaptorNoIsEnabled.IsEnabled(logger));

            var catAdaptor = new LogCategoryReflectionAdaptor("Debug", typeof(TestLogger));
            logger.IsDebugEnabled = null;
            Assert.IsTrue(catAdaptor.IsEnabled(logger));
            logger.IsDebugEnabled = false;
            Assert.IsFalse(catAdaptor.IsEnabled(logger));
            logger.IsDebugEnabled = true;
            Assert.IsTrue(catAdaptor.IsEnabled(logger));
            logger.IsDebugEnabled = new object();
            Assert.IsTrue(catAdaptor.IsEnabled(logger));
        }

        [TestMethod]
        public void LogAllFound()
        {
            var catAdaptor = new LogCategoryReflectionAdaptor("Debug", typeof(TestLogger));
            var logger = new TestLogger();
            catAdaptor.Log(logger, "A Message");
            catAdaptor.Log(logger, "A Message and Exception", new ArithmeticException("Test Exception"));
            catAdaptor.LogFormat(logger, "A Message and {0}, {1}", "Arguments", "List");
            catAdaptor.LogFormat(logger, CultureInfo.GetCultureInfo("de-DE"), "A Message and {0}, {1}", "Arguments", 1.1);
            Assert.AreEqual(@"Debug(""A Message"")
Debug(""A Message and Exception"", System.ArithmeticException: Test Exception)
Debug(""A Message and {0}, {1}"", object[2] = Arguments, List)
Debug(de-DE, ""A Message and {0}, {1}"", object[2] = Arguments, 1.1)", string.Join("\r\n", logger.Calls));
        }
    }
}