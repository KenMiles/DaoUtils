using System;
using DaoUtilsCore.code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.code
{
    [TestClass]
    public class CodeKeyTests
    {
        readonly byte[] _binaryData = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly string _binaryDataAsBase64 = "AQIDBAUGBwgJCg==";
        //asBase64Str = Convert.ToBase64String(binaryData)
        readonly CodeKey _codeKey = new CodeKey();

        [TestMethod]
        public void CheckGetKeyAsBase64String()
        {
            _codeKey.Key = _binaryData;
            Assert.AreEqual(_binaryDataAsBase64, _codeKey.KeyBase64);
        }

        [TestMethod]
        public void CheckSetKeyFromBase64String()
        {
            _codeKey.KeyBase64 = _binaryDataAsBase64;
            CollectionAssert.AreEqual(_binaryData, _codeKey.Key);
        }

        [TestMethod]
        public void CheckGetInitializationVectorAsBase64String()
        {
            _codeKey.InitializationVector = _binaryData;
            Assert.AreEqual(_binaryDataAsBase64, _codeKey.InitializationVectorBase64);
        }

        [TestMethod]
        public void CheckSetInitializationVectorFromBase64String()
        {
            _codeKey.InitializationVectorBase64 = _binaryDataAsBase64;
            CollectionAssert.AreEqual(_binaryData, _codeKey.InitializationVector);
        }
    }
}
