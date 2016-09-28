using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using DaoUtilsCore.log;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.standard
{
    class AccessDaoHelper : DaoHelper
    {
        public AccessDaoHelper(IDbConnection connection, OpenConnection openConnection, ILog log = null) : base(connection, openConnection, log)
        {
        }

        public IDaoCommand<IReadValue, IDbCommand> AccessCreateCommand(string commandText, DaoSetupParameters<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> setupParameters)
        {
            return CreateCommand(commandText, setupParameters, false);
        }

        public IDaoQuery<IReadValue, IDbCommand> AccessCreateQuery(string querySql, DaoSetupParameters<IDaoParametersBuilderInput> setupParameters)
        {
            return CreateQuery(querySql, setupParameters);
        }

    }

    [TestClass]
    public class DaoHelperTests
    {
        readonly Mock<IDbConnection> _connection = new Mock<IDbConnection>(MockBehavior.Loose);
        readonly Mock<ILog> _log = new Mock<ILog>(MockBehavior.Loose);
        readonly Mock<IDbCommand> _command = new Mock<IDbCommand>(MockBehavior.Strict);
        readonly Mock<IDataParameterCollection> _paramCollection = new Mock<IDataParameterCollection>();


        private AccessDaoHelper _uut;

        [TestInitialize]
        public void SetUp()
        {
            _uut = new AccessDaoHelper(_connection.Object, OpenConnection.FirstAccess, _log.Object);
        }

        private void SetupCreateCommand(string sql)
        {
            _connection.Setup(c => c.CreateCommand()).Returns(_command.Object);
            _command.SetupSet(c => c.CommandText = sql)
                .Callback<string>(cmd => _command.Setup(c => c.CommandText).Returns(sql))
                .Verifiable();
            _command.Setup(c => c.Parameters).Returns(_paramCollection.Object);
        }

        private Mock<IDbDataParameter> MockParameter(string name, ParameterDirection direction, int paramSize, DbType type)
        {
            var result = new Mock<IDbDataParameter>(MockBehavior.Strict);
            _command.Setup(s => s.CreateParameter()).Returns(result.Object);
            result.SetupSet(r => r.Size = paramSize).Verifiable();
            result.SetupSet(r => r.ParameterName = name).Verifiable();
            result.SetupSet(r => r.Direction = direction).Verifiable();
            result.SetupSet(r => r.DbType = type).Verifiable();
            _paramCollection.Setup(p => p.Add(result.Object)).Verifiable();
            _paramCollection.Setup(p => p.Clear()).Verifiable();
            return result;
        }


        [TestMethod]
        public void CreateCommand()
        {
            SetupCreateCommand("A Command @AParam");
            var param = MockParameter("AParam", ParameterDirection.ReturnValue, 1001, DbType.String);
            var cmd = _uut.AccessCreateCommand("A Command @AParam",
                helper => helper.Name("AParam").Size(1001).ReturnValue.AsStringParameter());
            Assert.IsNotNull(cmd);
            Assert.IsNotNull(cmd as DaoCommand);
            Assert.AreEqual(_command.Object, cmd.Command);
            _command.Verify();
            param.Verify();
            _paramCollection.Verify();
        }

        [TestMethod]
        public void CreateCommandLogsErrorCreating()
        {
            Exception e = new Exception("A Message");
            _connection.Setup(c => c.CreateCommand()).Throws(e);
            try
            {
                _uut.AccessCreateCommand("A Command", helper => { });
                Assert.Fail("Exception Expected");
            }
            catch (Exception ex)
            {
                Assert.AreSame(ex, e);
            }
            _log.Setup(l => l.Error($"Error Creating command: {"A Command"}", e));
        }

        [TestMethod]
        public void CreateAQuery()
        {
            SetupCreateCommand("A Query @AParam");
            var param = MockParameter("AParam", ParameterDirection.Input, 0, DbType.String);
            var cmd = _uut.AccessCreateQuery("A Query @AParam",
                helper => helper.Name("AParam").AsStringParameter());
            Assert.IsNotNull(cmd);
            Assert.IsNotNull(cmd as DaoCommand);
            Assert.AreEqual(_command.Object, cmd.Command);
            _command.Verify();
            param.Verify();
            _paramCollection.Verify();
        }

        [TestMethod]
        public void CreateQuerLogsErrorCreating()
        {
            Exception e = new Exception("A Message");
            _connection.Setup(c => c.CreateCommand()).Throws(e);
            try
            {
                _uut.AccessCreateQuery("A Command", helper => { });
                Assert.Fail("Exception Expected");
            }
            catch (Exception ex)
            {
                Assert.AreSame(ex, e);
            }
            _log.Setup(l => l.Error($"Error Creating command: {"A Command"}", e));
        }
    }
}
