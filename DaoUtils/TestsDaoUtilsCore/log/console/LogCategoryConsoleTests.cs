using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DaoUtilsCore.log;
using DaoUtilsCore.log.console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.log.console
{
    [TestClass]
    public class LogCategoryConsoleTests
    {
        private readonly Mock<IConsolWrapper> _consoleWrapper = new Mock<IConsolWrapper>(MockBehavior.Strict);

        private LogCategoryConsole CreateUut(string category, bool enabled, bool errorStream)
        {
            return new LogCategoryConsole(category, enabled, errorStream, _consoleWrapper.Object);
        }

        [TestMethod]
        public void IsEnabled()
        {
            Assert.IsTrue(CreateUut("Info", true, true).IsEnabled(null));
            Assert.IsTrue(CreateUut("Warn", true, true).IsEnabled(new object()));
            Assert.IsTrue(CreateUut("Debug", true, false).IsEnabled(null));
            Assert.IsTrue(CreateUut("Error", true, false).IsEnabled(new object()));
            Assert.IsFalse(CreateUut("Info", false,true).IsEnabled(null));
            Assert.IsFalse(CreateUut("Warn", false,true).IsEnabled(new object()));
            Assert.IsFalse(CreateUut("Debug", false,false).IsEnabled(null));
            Assert.IsFalse(CreateUut("Error", false,false).IsEnabled(new object()));
        }

        private string ExpectedEntry(string logger, string category, string message, Exception exception = null)
        {
            var result = $"{logger}: {category} {message}";
            if (exception != null) result += $"{Environment.NewLine}{exception}";
            return result;
        }


        private void CheckDatedLog(string expectedMessage, string actualMessage)
        {
            var parts = actualMessage.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
            Assert.IsTrue(parts.Length > 1, $"Msg = {actualMessage}");
            Assert.AreEqual(expectedMessage, string.Join("\t", parts.Skip(1)));
            var diff = DateTime.Now - DateTime.Parse(parts[0]);
            // If test takes more than second would be surprised 
            Assert.IsTrue(diff.TotalSeconds >= 0 && diff.TotalSeconds < 10, $"Time Diff is {diff}");
        }

        private void SetupExpectations(string expectedMessage, bool normalWrite = false, bool errorWrite = false)
        {
            _consoleWrapper
                .Setup(c => c.WriteLnToDebug(It.IsAny<string>()))
                .Callback<string>(m => Assert.AreEqual(expectedMessage, m))
                .Verifiable();
            if (normalWrite || errorWrite)
            {
                _consoleWrapper
                    .Setup(c => c.WriteLn(It.IsAny<string>(), It.IsAny<bool>()))
                    .Callback<string, bool>((m, e) =>
                    {
                        CheckDatedLog(expectedMessage, m);
                        Assert.AreEqual(errorWrite, e);
                    } )
                    .Verifiable();
            }
            else
            {
                _consoleWrapper
                    .Setup(c => c.WriteLn(It.IsAny<string>(), It.IsAny<bool>()))
                    .Callback<string, bool>((m, e) =>
                    {
                        Assert.Fail("Shouldn't be here");
                    });
            }
        }

        private void CheckLogger(Action<LogCategoryConsole> writeLog, string category, string logger, string message, bool enabled, bool errorStream, Exception exception = null)
        {
            SetupExpectations(ExpectedEntry(logger, category, message, exception), enabled, errorStream);
            writeLog(CreateUut(category, enabled, errorStream));
            _consoleWrapper.Verify();
        }

        private void CheckLogger(string category, string logger, string message, bool enabled, bool errorStream)
        {
            CheckLogger(h => h.Log(logger, message), category, logger, message, enabled, errorStream);
        }

        private void CheckLogger(string category, string logger, string message, bool enabled, bool errorStream, Exception exception)
        {
            CheckLogger(h => h.Log(logger, message, exception), category, logger, message, enabled, errorStream, exception);
        }

        private Exception TestException()
        {
            try {
               throw new Exception();
            }
            catch(Exception e)
            {
                return e;
            }
        }

        [TestMethod]
        public void LogMessage()
        {
            CheckLogger("A Category", "A Logger", "A message", false, false);
            CheckLogger("A Different Category", "A Different Logger", "A Different message", true, false);
            CheckLogger("Yet Another Category", "Yet Another Logger", "Yet Another message", true, true);
        }

        [TestMethod]
        public void LogMessageAndException()
        {
            CheckLogger("A Category", "A Logger", "A message", false, false, TestException());
            CheckLogger("A Different Category", "A Different Logger", "A Different message", true, false, TestException());
            CheckLogger("Yet Another Category", "Yet Another Logger", "Yet Another message", true, true, TestException());
        }


        [TestMethod]
        public void LogFormat()
        {
            CheckLogger(h => h.LogFormat("A logger", "Message {0}", 1.01), "The Category", "A logger", "Message 1.01", false, false);
            CheckLogger(h => h.LogFormat("A logger", "Message {0}", 2.02), "The Category", "A logger", "Message 2.02", true, false);
            CheckLogger(h => h.LogFormat("A logger", "Message {0}", 3.03), "The Category", "A logger", "Message 3.03", true, true);
        }

        [TestMethod]
        public void LogFormatUsingProvider()
        {
            var cultureCommaAsDecSep = new CultureInfo("de-DE");
            CheckLogger(h => h.LogFormat("A logger", cultureCommaAsDecSep, "Message {0}", 1.01), "The Category", "A logger", "Message 1,01", false, false);
            CheckLogger(h => h.LogFormat("A logger", cultureCommaAsDecSep, "Message {0}", 2.02), "The Category", "A logger", "Message 2,02", true, false);
            CheckLogger(h => h.LogFormat("A logger", cultureCommaAsDecSep, "Message {0}", 3.03), "The Category", "A logger", "Message 3,03", true, true);
        }
    }
}
