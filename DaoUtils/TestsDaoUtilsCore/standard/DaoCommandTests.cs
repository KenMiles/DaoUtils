using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.standard
{
    internal class DaoCommandAccess : DaoCommand
    {
        public DaoCommandAccess(IDbCommand command, IDaoConnectionInfo connectionInfo) : base(command, connectionInfo)
        {
        }

        public List<IDaoParameterInternal> AccessParameters => Parameters;

        public IReadHelper<IReadValue> AccessReadHelper(List<IDaoParameterInternal> parameters)
        {
            return ReadHelper(parameters);
        }

        public IReadHelper<IReadValue> AccessReadHelper(IDataReader dataReader)
        {
            return ReadHelper(dataReader);
        }
    }

    [TestClass]
    public class DaoCommandTests
    {
        private readonly Mock<IDbCommand> _dbCmd = new Mock<IDbCommand>(MockBehavior.Strict);
        private readonly Mock<IDaoConnectionInfo> _info = new Mock<IDaoConnectionInfo>(MockBehavior.Strict);
        private DaoCommandAccess _command;

        [TestInitialize]
        public void Setup()
        {
            _command = new DaoCommandAccess(_dbCmd.Object, _info.Object);
        }

        [TestMethod]
        public void ParameterBuilderName()
        {
            IDaoParametersBuilderDirection<
                IDaoParametersBuilderInput, 
                IDaoParametersBuilderInputOutput, 
                IDaoParametersBuilderOutput
            > builder = _command.Name("A Name");
            Assert.IsNotNull(builder);
            Assert.AreEqual(0, _command.AccessParameters?.Count);
            // test creating parameter adds to list (ie correct list passed)
            _dbCmd.Setup(c => c.CreateParameter()).Returns(new Mock<IDbDataParameter>().Object);
            var parameter = builder.Output.AsByteParameter();
            Assert.AreEqual(1, _command.AccessParameters?.Count);
            Assert.AreSame(parameter, _command.AccessParameters?[0]);
        }

        private readonly string[] _names = {"Zero", "One", "Two", "Three", "Four", "Five"};

        [TestMethod]
        public void ParameterReadHelper()
        {
            var paramaters = _names.Select(n =>
            {
                var p = new Mock<IDaoParameterInternal>();
                p.Setup(m => m.Name).Returns(n);
                return p;
            }).ToArray();
            IReadHelper<IReadValue> helper = _command.AccessReadHelper(paramaters.Select(p => p.Object).ToList());
            Assert.IsNotNull(helper as ParametersReadHelper);
            for (var idx = 0; idx < paramaters.Length; idx++)
            {
                var val = $"{idx} - {_names[idx]}";
                paramaters[idx].Setup(p => p.GetValueAsObject()).Returns(val);
                Assert.AreEqual(val, helper.Named(_names[idx]).AsString);
                Assert.AreEqual(val, helper[idx].AsString);
            }

        }

        [TestMethod]
        public void DataReaderReadHelper()
        {
            Mock< IDataReader> dataRead = new Mock<IDataReader>(MockBehavior.Loose);
            dataRead.Setup(dr => dr.FieldCount).Returns(_names.Length);
            for (var idx = 0; idx < _names.Length; idx++)
            {
                var col = idx;
                dataRead.Setup(dr => dr.GetName(col)).Returns(_names[col]);
            }
            IReadHelper<IReadValue> helper = _command.AccessReadHelper(dataRead.Object);
            Assert.IsNotNull(helper as DataReaderHelper);
            for (var idx = 0; idx < _names.Length; idx++)
            {
                var val = $"{idx} - {_names[idx]}";
                var col = idx;
                dataRead.Setup(dr => dr.GetString(col)).Returns(val);
                Assert.AreEqual(val, helper.Named(_names[idx]).AsString);
                Assert.AreEqual(val, helper[idx].AsString);
            }
        }
    }
}
