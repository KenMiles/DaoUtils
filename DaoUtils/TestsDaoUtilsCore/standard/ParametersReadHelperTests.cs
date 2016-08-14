using System;
using System.Collections.Generic;
using System.Linq;
using DaoUtils.core;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.standard
{
    class AccessParametersReadHelper: ParametersReadHelper
    {
        public AccessParametersReadHelper(IEnumerable<IDaoParameterInternal> parameters) : base(parameters)
        {
        }

        public IReadValue AccessParamReader(IDaoParameterInternal parameter)
        {
            return ParamReader(parameter);
        }
    }

    [TestClass]
    public class ParametersReadHelperTests
    {
        private readonly Mock<IDaoParameterInternal> _param = new Mock<IDaoParameterInternal>(MockBehavior.Strict);
        private readonly string[] _names = { "Zero", "One", "Two", "Three", "Four", "Five" };
        private Mock<IDaoParameterInternal>[] _parameters;
        private AccessParametersReadHelper _uut;

        [TestInitialize]
        public void SetUp()
        {
            _parameters = _names.Select(n =>
            {
                var p = new Mock<IDaoParameterInternal>(MockBehavior.Strict);
                p.Setup(m => m.Name).Returns(n);
                return p;
            }).ToArray();
            _uut = new AccessParametersReadHelper(_parameters.Select(p => p.Object));
        }


        [TestMethod]
        public void ParamReader()
        {
            var reader = _uut.AccessParamReader(_param.Object);
            Assert.IsNotNull(reader);
            Assert.IsNotNull(reader as ParameterReadValue);
            _param.Setup(p => p.GetValueAsObject()).Returns("Not Null").Verifiable();
            Assert.IsFalse(reader.DbNull);
            _param.Verify();
        }
    }
}
