using System;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.core
{
    [TestClass]
    public class DataTypeAdaptorTests
    {
        [TestMethod]
        public void NullReturnsDefaultForClass()
        {
            Assert.AreEqual(null, DataTypeAdaptor.ConvertValue<string>(null));
            Assert.AreEqual(default(int), DataTypeAdaptor.ConvertValue<int>(null));
            Assert.AreEqual(default(double), DataTypeAdaptor.ConvertValue<double>(null));
        }

        [TestMethod]
        public void NullReturnsDefaultPassed()
        {
            Assert.AreEqual("A Default", DataTypeAdaptor.ConvertValue<string>(null, "A Default"));
            Assert.AreEqual(991, DataTypeAdaptor.ConvertValue<int>(null, 991));
            Assert.AreEqual(-1021.1, DataTypeAdaptor.ConvertValue<double>(null, -1021.1));
        }

        [TestMethod]
        public void ConvertSameType()
        {
            Assert.AreEqual("A Default", DataTypeAdaptor.ConvertValue<string>("A Default", "Not This"));
            Assert.AreEqual(991, DataTypeAdaptor.ConvertValue<int>(991, 11221));
            Assert.AreEqual(-1021.1, DataTypeAdaptor.ConvertValue<double>(-1021.1, 121212));
        }

        [TestMethod]
        public void ConvertAssignable()
        {
            Assert.AreEqual(991, DataTypeAdaptor.ConvertValue<int>(991.0));
            Assert.AreEqual(102, DataTypeAdaptor.ConvertValue<double>(102));
        }


        [TestMethod]
        public void ConvertConvertable()
        {
            Assert.AreEqual(991, DataTypeAdaptor.ConvertValue<int>("991"));
            Assert.AreEqual(102.12, DataTypeAdaptor.ConvertValue<double>("102.12"));
        }

        [TestMethod]
        public void NoConversionAvialable()
        {
            try
            {
                DataTypeAdaptor.ConvertValue<int>(new object());
                Assert.Fail("Exception Expected");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreEqual($"Don't know how to convert'{typeof(object)}' to {typeof(int)}", e.Message);
            }

        }
    }
}
