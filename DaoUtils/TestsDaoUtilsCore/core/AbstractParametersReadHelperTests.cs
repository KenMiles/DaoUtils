using System;
using System.Collections.Generic;
using System.Linq;
using DaoUtils.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using Moq;
using TestsDaoUtilsCore.testHelpers;

namespace TestsDaoUtilsCore.core
{
    internal class TestableAbstractParametersReadHelper : AbstractParametersReadHelper<IReadValue>
    {
        public Dictionary<IDaoParameterInternal, IReadValue> Returned { get; } = new Dictionary<IDaoParameterInternal, IReadValue>();
        public TestableAbstractParametersReadHelper(IEnumerable<IDaoParameterInternal> parameters) : base(parameters)
        {
        }

        protected override IReadValue ParamReader(IDaoParameterInternal parameter)
        {
            var resut = new Mock<IReadValue>(MockBehavior.Strict);
            Returned.Add(parameter, resut.Object);
            return resut.Object;
        }
    }

    [TestClass]
    public class AbstractParametersReadHelperTests
    {

        private static Mock<IDaoParameterInternal> MockParam(string paramName)
        {
            var result = new Mock<IDaoParameterInternal>(MockBehavior.Strict);
            result.Setup(m => m.Name).Returns(paramName).Verifiable();
            return result;
        }

        private static Mock<IDaoParameterInternal>[] MockParams(params string[] paramName)
        {
            return paramName.Select(MockParam).ToArray();
        }

        private static readonly string[] ParamNames = { "One", "Two", "Three", "Four", "Five" };
        readonly private Mock<IDaoParameterInternal>[] _parameters = MockParams(ParamNames);
        private TestableAbstractParametersReadHelper _testing;

        [TestInitialize]
        public void SetUp()
        {
            _testing = new TestableAbstractParametersReadHelper(_parameters.Select(p => p.Object));
        }

        [TestMethod]
        public void ConstructorCallsParamReaderForAll()
        {
            Array.ForEach(_parameters, p => p.Verify());
            Assert.AreEqual(_parameters.Length, _testing.Returned.Count);
            Assert.AreEqual(_parameters.Length, _testing.Returned.Values.Where(v => v != null).Distinct().Count());
            CheckArrays.CheckSameValues("Keys Match expected", _testing.Returned.Keys, _parameters.Select(p => p.Object));
        }

        [TestMethod]
        public void NamedReturnsCorrectReader()
        {
            foreach (var param in _parameters)
            {
                Assert.AreEqual(_testing.Returned[param.Object], _testing.Named(param.Object.Name));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaoUtilsException), "Unknown Parameter 'Hello'")]
        public void NamedThrowsExcetionUnknownName()
        {
            _testing.Named("Hello");
        }

        [TestMethod]
        [ExpectedException(typeof(DaoUtilsException), "Unknown Parameter ''")]
        public void NamedThrowsExcetionNullName()
        {
            _testing.Named(null);
        }

        [TestMethod]
        public void IndexedByNameReturnsCorrectReader()
        {
            foreach (var param in _parameters)
            {
                Assert.AreEqual(_testing.Returned[param.Object], _testing[param.Object.Name]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DaoUtilsException), "Unknown Parameter 'Hello'")]
        public void IndexedByNamedThrowsExcetionUnknownName()
        {
            var test = _testing["Hello"];
        }

        [TestMethod]
        [ExpectedException(typeof(DaoUtilsException), "Unknown Parameter ''")]
        public void IndexedByNamedThrowsExcetionNullName()
        {
            var test = _testing[null];
        }

        [TestMethod]
        public void IndexedByIdxReturnsCorrectReader()
        {
            var readHlper = (IReadHelper<IReadValue>) _testing;
            for (var idx =0; idx < _parameters.Length; idx++)
            {
                Assert.AreEqual(_testing.Returned[_parameters[idx].Object], readHlper[idx]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void IndexedByIdxThrowsExcetionOutOfRange()
        {
            var readHlper = (IReadHelper<IReadValue>)_testing;
            var test = readHlper[99];
        }
    }
}
