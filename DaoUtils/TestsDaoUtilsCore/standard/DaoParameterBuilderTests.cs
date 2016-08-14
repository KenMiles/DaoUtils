using System;
using System.Collections.Generic;
using System.Data;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.standard
{

    [TestClass]
    public class DaoParameterBuilderTests
    {
        readonly Mock<IDbCommand> _command = new Mock<IDbCommand>(MockBehavior.Strict);
        readonly List<IDaoParameterInternal> _parameters = new List<IDaoParameterInternal>();
        private const string ParamName = "A Name";

        private DaoParameterBuilder _uut;
        [TestInitialize]
        public void SetUp()
        {
            _uut = new DaoParameterBuilder(_command.Object, _parameters, ParamName);
        }

        private Mock<IDbDataParameter> MockParameter(string name, ParameterDirection direction, int paramSize, DbType type)
        {
            var result = new Mock<IDbDataParameter>(MockBehavior.Strict);
            _command.Setup(s => s.CreateParameter()).Returns(result.Object);
            result.SetupSet(r => r.Size = paramSize).Verifiable();
            result.SetupSet(r => r.ParameterName = name).Verifiable();
            result.SetupSet(r => r.Direction = direction).Verifiable();
            result.SetupSet(r => r.DbType = type).Verifiable();
            return result;
        }

        [TestMethod]
        public void CreateAParameter()
        {
            var param = MockParameter(ParamName, ParameterDirection.ReturnValue, 9921, DbType.Binary);
            var built = _uut.Size(9921).ReturnValue.AsBinaryParameter();
            Assert.AreEqual(1, _parameters.Count);
            Assert.AreSame(built, _parameters[0]);
            param.Verify();
        }
    }
}
