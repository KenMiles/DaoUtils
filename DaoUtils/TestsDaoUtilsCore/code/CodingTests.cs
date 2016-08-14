using System;
using System.Collections.Generic;
using DaoUtils.code;
using DaoUtilsCore.code;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.code
{
    [TestClass]
    public class CodingTests
    {
        private const string InitialStr = "b6D4GEWjkw4Yp/X9sPebNkzuNzehDcnQYgeF1a3Rza3cV1YwsP/I5NvtzlDcKaNA92V4CEawoAQ5koWXUx7mqnKhtqu/YtHAAM4g+lgKDYU=";
        private const string InitialStr2 = "zc7YUrJgPultm/5OnReu1PtGniARU7s8kENDDBZ28ZPdXnzBag0UDwZCJ/pJQ0w+hTVfsKvDv/56sGsUr5flLLJEaBwL7CHmXr0rYZT4gdg=";
        readonly private Coding _coding = new Coding(InitialStr);
        readonly private Coding _coding2 = new Coding(InitialStr2);

        private const string ValidEncryptedPassword = "hRc/D5wBhHo0Kmq4oMo6OESpTlO99cavjsGxrNPmJF3VUHo2kwKq9VLqC8eY1HljvYC6BazLJMCtX9/rzYUZJg==";

        [TestMethod]
        public void DecryptAPassword()
        {
            var decoded =_coding.DecryptString(ValidEncryptedPassword);
            Assert.AreEqual("A Test  password", decoded);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Security.Cryptography.CryptographicException))]
        public void DecryptPasswordWrongInitializationStr()
        {
            _coding2.DecryptString(ValidEncryptedPassword);
        }

        [TestMethod]
        public void EncryptAndDecryptAPassword()
        {
            const string aPassword = "This 123 Password";
            var encrypted = _coding.EncryptStr(aPassword);
            var decoded = _coding.DecryptString(encrypted);
            Assert.AreEqual(aPassword, decoded);
            //Just some simple checks to show not same, but very superficial 
            Assert.AreNotEqual(aPassword, encrypted);
            Assert.IsFalse(encrypted.Contains(aPassword));
            Assert.AreNotEqual(aPassword.Length, encrypted.Length);
        }

        [TestMethod]
        public void EncryptPasswordProducesDifferentResultEachTime()
        {
            // very slight possibly this will fail because may produce same random salt, but very small
            const string aPassword = "A different Password";
            var dic = new Dictionary<string, string>();
            for (int i = 0; i < 100; i++)
            {
               dic.Add(_coding.EncryptStr(aPassword), aPassword);
                dic.Add(_coding2.EncryptStr(aPassword), aPassword);
            }
            Assert.AreEqual(200, dic.Count);
        }
    }
}
