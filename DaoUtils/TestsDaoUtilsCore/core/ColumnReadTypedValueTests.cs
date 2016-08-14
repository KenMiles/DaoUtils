using System;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.core
{
    [TestClass]
    public class ColumnReadTypedValueTests
    {
        private bool IsDbNull { get; set; } = false;
        private object ReturnedObject { get; set; } = null;
        private ColumnReadTypedValue<object> _testing;

        [TestInitialize]
        public void SetUp()
        {
            _testing = new ColumnReadTypedValue<object>(()=> IsDbNull, () => ReturnedObject);
        }

        [TestMethod]
        public void ReadWhenDbNull()
        {
            IsDbNull = true;
            ReturnedObject = new object();
            Assert.IsNull(_testing.Value);
            Assert.IsNull(_testing.Read());
            var defaultValue = new object();
            Assert.AreSame(defaultValue, _testing.Read(defaultValue));
        }

        [TestMethod]
        public void ReadWhenNotDbNull()
        {
            IsDbNull = false;
            var defaultValue = new object();
            var returnedObject = ReturnedObject = new object();
            Assert.AreSame(returnedObject, _testing.Value);
            Assert.AreSame(returnedObject, _testing.Read());
            Assert.AreSame(returnedObject, _testing.Read(defaultValue));
            // Now check not caching
            var returnedObject2 = ReturnedObject = new object();
            Assert.AreNotSame(returnedObject2, returnedObject);
            Assert.AreSame(returnedObject2, _testing.Value);
            Assert.AreSame(returnedObject2, _testing.Read());
            Assert.AreSame(returnedObject2, _testing.Read(defaultValue));
        }
    }
}
