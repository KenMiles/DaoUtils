using System;
using DaoUtilsCore.log;
using DaoUtilsCore.log.reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogManager = DaoUtilsCore.log.console.LogManager;

namespace TestsDaoUtilsCore.log.console
{
    [TestClass]
    public class LogManagerTests
    {
        [TestMethod]
        public void InstanceNotNull()
        {
            Assert.IsNotNull(LogManager.Instance);
        }

        private void CheckIsLogAdaptorExcerciseAllCategories(ILog logger)
        {
            Assert.IsInstanceOfType(logger, typeof(LogAdaptor));
            // Not going to try testing these work (could possibly via console redirections) - just that they don't throw an error which means Catgory isn't null
            logger.Error("Error");
            logger.Debug("Debug");
            logger.Info("Info");
            logger.Fatal("Fatal");
            logger.Warn("Warn");
        }


        [TestMethod]
        public void GetFromNameReturnsLogAdaptor()
        {
            CheckIsLogAdaptorExcerciseAllCategories(LogManager.Instance.GetLogger("A Log Name"));
        }

        [TestMethod]
        public void GetFromTypeReturnsLogAdaptor()
        {
            CheckIsLogAdaptorExcerciseAllCategories(LogManager.Instance.GetLogger(typeof(LogManagerTests)));
        }


    }
}
