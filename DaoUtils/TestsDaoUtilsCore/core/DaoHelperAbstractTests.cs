using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Common.Logging;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    internal class CommandRequest
    {
        public string CommandText { get; set; }

        public
            DaoSetupParameters
                <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>
            SetupParameters { get; set; }
    }

    internal class QueryRequest
    {
        public string QuerySql { get; set; }
        public DaoSetupParameters<IDaoParametersBuilderInput> SetupParameters { get; set; }
    }

    internal class TestableDaoHelperAbstract :
        DaoHelperAbstract
            <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput, IReadValue,
                IDbCommand>
    {

        public TestableDaoHelperAbstract(IDbConnection connection, OpenConnection openConnection, ILog log)
            : base(connection, openConnection, log)
        {
        }

        public List<CommandRequest> CommandRequests = new List<CommandRequest>();

        public Mock<IDaoCommand<IReadValue, IDbCommand>> MockCommand =
            new Mock<IDaoCommand<IReadValue, IDbCommand>>(MockBehavior.Strict);


        protected override IDaoCommand<IReadValue, IDbCommand> CreateCommand(string commandText,
            DaoSetupParameters
                <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>
                setupParameters)
        {
            CommandRequests.Add(new CommandRequest() {CommandText = commandText, SetupParameters = setupParameters});
            return MockCommand.Object;
        }

        public List<QueryRequest> QueryRequests = new List<QueryRequest>();

        public Mock<IDaoQuery<IReadValue, IDbCommand>> MockQuery =
            new Mock<IDaoQuery<IReadValue, IDbCommand>>(MockBehavior.Strict);

        protected override IDaoQuery<IReadValue, IDbCommand> CreateQuery(string querySql,
            DaoSetupParameters<IDaoParametersBuilderInput> setupParameters)
        {
            QueryRequests.Add(new QueryRequest() {QuerySql = querySql, SetupParameters = setupParameters});
            return MockQuery.Object;
        }

        public string AccessGetConnectionDescription(IDbConnection connection)
        {
            return GetConnectionDescription(connection);
        }

        public IDbConnection AccessConnection => Connection;
    }

    [TestClass]
    public class DaoHelperAbstractTests
    {
        private readonly Mock<IDbConnection> _connection = new Mock<IDbConnection>(MockBehavior.Strict);
        private readonly Mock<ILog> _log = new Mock<ILog>(MockBehavior.Loose);
        private TestableDaoHelperAbstract _uut;

        private OpenConnection _openConnectionType = OpenConnection.FirstAccess;

        private void SetupConnectionState(ConnectionState connectionState = ConnectionState.Open)
        {
            _connection.Setup(c => c.State).Returns(connectionState);
        }

        private TestableDaoHelperAbstract CreateUut()
        {
            return new TestableDaoHelperAbstract(_connection.Object, _openConnectionType, _log.Object);
        }

        private TestableDaoHelperAbstract Uut => _uut = _uut ?? CreateUut();

        private TestableDaoHelperAbstract OpenUut()
        {
            SetupConnectionState(ConnectionState.Open);
            return Uut;
        }

        [TestMethod]
        public void GetConnectionDescription()
        {
            Assert.AreEqual("Connection is null", Uut.AccessGetConnectionDescription(null));
            _connection.Setup(c => c.Database).Returns((string) null);
            Assert.AreEqual("unknown database", Uut.AccessGetConnectionDescription(_connection.Object));
            _connection.Setup(c => c.Database).Returns("  ");
            Assert.AreEqual("unknown database", Uut.AccessGetConnectionDescription(_connection.Object));
            _connection.Setup(c => c.Database).Returns("A Database Connection");
            Assert.AreEqual("A Database Connection", Uut.AccessGetConnectionDescription(_connection.Object));
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ConstructorExceptionIfNullConnection()
        {
            var test = new TestableDaoHelperAbstract(null, OpenConnection.FirstAccess, _log.Object);
        }

        [TestMethod]
        public void ConstructorDeferOpeningConnection()
        {
            _openConnectionType = OpenConnection.FirstAccess;
            Assert.IsNotNull(Uut);
            _connection.Verify(c => c.Open(), Times.Never);
            Assert.AreEqual(_connection.Object, Uut.AccessConnection);
        }

        [TestMethod]
        public void ConstructorOpeningConnectionImmediately()
        {
            _openConnectionType = OpenConnection.Immediate;
            _connection.Setup(c => c.Open());
            Assert.IsNotNull(Uut);
            _connection.Verify(c => c.Open(), Times.Exactly(1));
            Assert.AreEqual(_connection.Object, Uut.AccessConnection);
        }

        [TestMethod]
        public void ConstructorOpeningConnectionBackground()
        {
            _openConnectionType = OpenConnection.Background;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            var connectionOpened = false;
            _connection.Setup(c => c.Open()).Callback(() =>
            {
                autoEvent.WaitOne(1000);
                connectionOpened = true;
            });
            Assert.IsNotNull(Uut);
            Assert.IsFalse(connectionOpened);
            Assert.AreEqual(_connection.Object, Uut.AccessConnection);
            // Tell open (in thread) to run then sleep to ensure it has chance before we continue
            autoEvent.Set();
            Thread.Sleep(100);
            Assert.IsTrue(connectionOpened);
            _connection.Verify(c => c.Open(), Times.Exactly(1));
        }

        [TestMethod]
        public void Dispose()
        {
            _openConnectionType = OpenConnection.FirstAccess;
            _connection.Setup(c => c.Dispose()).Verifiable();
            IDisposable helper = Uut as IDisposable;
            Assert.IsNotNull(helper);
            helper.Dispose();
            _connection.Verify();
            helper.Dispose();
            _connection.Verify(c => c.Dispose(), Times.Exactly(1));
        }

        private void CheckErrorsAfterDispose(Action<TestableDaoHelperAbstract> action)
        {
            _openConnectionType = OpenConnection.FirstAccess;
            _connection.Setup(c => c.Dispose()).Verifiable();
            Uut.Dispose();
            try
            {
                action(Uut);
                Assert.Fail("Should have thrown Exception");
            }
            catch (DaoUtilsException ex)
            {
                Assert.AreEqual("Disposed Called - No longer able to call database", ex.Message);
            }
        }

        [TestMethod]
        public void WaitOpenAfterDispose()
        {
            CheckErrorsAfterDispose(uut => uut.WaitOpen());
        }

        [TestMethod]
        public void NewQueryAfterDispose()
        {
            CheckErrorsAfterDispose(uut => uut.NewQuery("Sql"));
        }

        [TestMethod]
        public void NewCommandAfterDispose()
        {
            CheckErrorsAfterDispose(uut => uut.NewCommand("Sql"));
        }

        [TestMethod]
        public void DoInTransactionAfterDispose()
        {
            CheckErrorsAfterDispose(uut => uut.DoInTransaction("Sql", () => { }));
        }

        [TestMethod]
        public void WaitOpenAlreadyOpen()
        {
            _connection.Setup(c => c.State).Returns(ConnectionState.Open).Verifiable();
            Uut.WaitOpen();
            _connection.Verify();
        }

        [TestMethod]
        public void WaitOpenClosedOpenFirstAccess()
        {
            _openConnectionType = OpenConnection.FirstAccess;
            SetupConnectionState(ConnectionState.Closed);
            _connection.Setup(c => c.Open()).Verifiable();
            Uut.WaitOpen();
            _connection.Verify();
        }

        [TestMethod]
        public void WaitOpenClosedOpenBackground()
        {
            _openConnectionType = OpenConnection.Background;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            var connectionOpened = false;
            _connection.Setup(c => c.Open()).Callback(() =>
            {
                autoEvent.WaitOne(1000);
                Thread.Sleep(100);
                connectionOpened = true;
            });
            SetupConnectionState(ConnectionState.Closed);
            autoEvent.Set();
            Uut.WaitOpen();
            Assert.IsTrue(connectionOpened);
        }

        [TestMethod]
        public void WaitOpenOpenBackgroundWhenErrorOpening()
        {
            _openConnectionType = OpenConnection.Background;
            var exception = new Exception("A Test");
            _connection.Setup(c => c.Open()).Throws(exception);
            SetupConnectionState(ConnectionState.Closed);
            try
            {
                Uut.WaitOpen();
                Assert.Fail("Should have thrown Exception");
            }
            catch (DaoUtilsException ex)
            {
                Assert.AreEqual("Opening Connection Threw Error", ex.Message);
                Assert.AreEqual(exception, ex.InnerException?.InnerException ?? ex.InnerException);
            }
        }

        [TestMethod]
        public void ParamPrefix()
        {
            // As would need to create a DBConnection classnot worth attempting to test more than just default if not DBConnection
            Assert.AreEqual("@", OpenUut().ParamPrefix);
        }

        private readonly
            DaoSetupParameters
                <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>
            _setupCommandParameters =
                helper => { Assert.IsNotNull(helper.Name("x")); /*Make sure not confused null setup*/ };

        private void VerifyCommandSetupParams(string sql)
        {
            Assert.AreEqual(1, Uut.CommandRequests.Count);
            var cr = Uut.CommandRequests[0];
            Assert.AreEqual(sql, cr.CommandText);
            Assert.AreEqual(_setupCommandParameters, cr.SetupParameters);
        }

        private void VerifyCommandEmptySetupParams(string sql)
        {
            Assert.AreEqual(1, Uut.CommandRequests.Count);
            var cr = Uut.CommandRequests[0];
            Assert.AreEqual(sql, cr.CommandText);
            Assert.IsNotNull(cr.SetupParameters);
            var helper =
                new Mock
                    <
                        IDaoSetupParametersHelper
                            <IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>>
                    (MockBehavior.Strict);
            cr.SetupParameters(helper.Object);
            helper.Verify();
        }

        [TestMethod]
        public void NewCommandWithParams()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            Assert.AreEqual(uut.MockCommand.Object, uut.NewCommand(sql, _setupCommandParameters));
            VerifyCommandSetupParams(sql);
        }

        [TestMethod]
        public void NewCommandWithoutParams()
        {
            const string sql = "Some Different SQL To Test";
            var uut = OpenUut();
            Assert.AreEqual(uut.MockCommand.Object, uut.NewCommand(sql));
            VerifyCommandEmptySetupParams(sql);
        }

        private readonly DaoSetupParameters<IDaoParametersBuilderInput> _setupQueryParameters =
            helper => { Assert.IsNotNull(helper.Name("x")); /*Make sure not confused null setup*/ };

        private void VerifyQuerySetupParams(string sql)
        {
            Assert.AreEqual(1, Uut.QueryRequests.Count);
            var cr = Uut.QueryRequests[0];
            Assert.AreEqual(sql, cr.QuerySql);
            Assert.AreEqual(_setupQueryParameters, cr.SetupParameters);
        }

        private void VerifyQueryEmptySetupParams(string sql)
        {
            Assert.AreEqual(1, Uut.QueryRequests.Count);
            var cr = Uut.QueryRequests[0];
            Assert.AreEqual(sql, cr.QuerySql);
            Assert.IsNotNull(cr.SetupParameters);
            var helper = new Mock<IDaoSetupParametersHelper<IDaoParametersBuilderInput>>(MockBehavior.Strict);
            cr.SetupParameters(helper.Object);
            helper.Verify();
        }

        [TestMethod]
        public void NewQueryWithParams()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            Assert.AreEqual(uut.MockQuery.Object, uut.NewQuery(sql, _setupQueryParameters));
            VerifyQuerySetupParams(sql);
        }

        [TestMethod]
        public void NewQueryWithoutParams()
        {
            const string sql = "Some Different SQL To Test";
            var uut = OpenUut();
            Assert.AreEqual(uut.MockQuery.Object, uut.NewQuery(sql));
            VerifyQueryEmptySetupParams(sql);
        }

        [TestMethod]
        public void DoInTransaction()
        {
            var uut = OpenUut();
            Assert.IsNull(uut.ActiveTransaction);
            var activeTransaction = new Mock<IDbTransaction>(MockBehavior.Strict);
            var calls = new List<string>();
            _connection.Setup(c => c.BeginTransaction()).Returns(activeTransaction.Object);
            activeTransaction.Setup(t => t.Commit()).Callback(() => calls.Add("Commit"));
            activeTransaction.Setup(t => t.Dispose()).Callback(() => calls.Add("Dispose"));
            uut.DoInTransaction("Test Trans", () =>
            {
                Assert.AreEqual(activeTransaction.Object, uut.ActiveTransaction);
                calls.Add("Do in transaction");
            });
            Assert.IsNull(uut.ActiveTransaction);
            Assert.AreEqual("Do in transaction,Commit,Dispose", string.Join(",", calls));
        }

        [TestMethod]
        public void DoInTransactionRollsBackWithError()
        {
            var uut = OpenUut();
            Assert.IsNull(uut.ActiveTransaction);
            var activeTransaction = new Mock<IDbTransaction>(MockBehavior.Strict);
            var calls = new List<string>();
            _connection.Setup(c => c.BeginTransaction()).Returns(activeTransaction.Object);
            activeTransaction.Setup(t => t.Rollback()).Callback(() => calls.Add("Rollback"));
            activeTransaction.Setup(t => t.Dispose()).Callback(() => calls.Add("Dispose"));
            Exception ex = new Exception("Testing");
            try
            {
                uut.DoInTransaction("Test Trans", () =>
                {
                    Assert.AreEqual(activeTransaction.Object, uut.ActiveTransaction);
                    calls.Add("Do in transaction");
                    throw ex;
                });
                Assert.Fail("Should have thrown Exception");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreSame(ex, e.InnerException);
                Assert.AreEqual($"{"Test Trans"} errored with '{"Testing"}'", e.Message);
            }
            Assert.IsNull(uut.ActiveTransaction);
            Assert.AreEqual("Do in transaction,Rollback,Dispose", string.Join(",", calls));
            _log.Verify(l => l.Error(ex));
        }

        [TestMethod]
        public void DoInTransactionErrorsIfAlreadyInTransaction()
        {
            var uut = OpenUut();
            Assert.IsNull(uut.ActiveTransaction);
            var activeTransaction = new Mock<IDbTransaction>(MockBehavior.Strict);
            var calls = new List<string>();
            _connection.Setup(c => c.BeginTransaction()).Returns(activeTransaction.Object);
            activeTransaction.Setup(t => t.Rollback()).Callback(() => calls.Add("Rollback"));
            activeTransaction.Setup(t => t.Dispose()).Callback(() => calls.Add("Dispose"));
            try
            {
                uut.DoInTransaction("Test Trans", () =>
                {
                    Assert.AreEqual(activeTransaction.Object, uut.ActiveTransaction);
                    calls.Add("Do in transaction");
                    uut.DoInTransaction("Inner Trans", () => { });
                });
                Assert.Fail("Should have thrown Exception");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreEqual($"{"Test Trans"} errored with '{"Already in Transaction"}'", e.Message);
                _log.Verify(l => l.Error(e.InnerException));
                Assert.AreEqual(typeof (DaoUtilsException), e.InnerException.GetType());
            }
            Assert.IsNull(uut.ActiveTransaction);
            Assert.AreEqual("Do in transaction,Rollback,Dispose", string.Join(",", calls));
        }

        private readonly DaoReadRow<string, IReadValue> _read = helper => helper[1].AsAnsiString;

        private readonly DaoReadRowAndParams<string, IReadValue> _readAndParams =
            (helper, parameterHelper) => parameterHelper["One"].AsString;

        private readonly List<string> _strings = new List<string>() {"One", "Two", "Three", "Four", "Five"};

        private void SetUpReadQuery(Mock<IDaoQuery<IReadValue, IDbCommand>> query)
        {
            query.Setup(c => c.ReadQuery(_read)).Returns(_strings).Verifiable();
            query.Setup(c => c.Dispose()).Verifiable();
        }

        private void SetUpReadRowAndParamsQuery(Mock<IDaoQuery<IReadValue, IDbCommand>> query)
        {
            query.Setup(c => c.ReadQuery(_readAndParams)).Returns(_strings).Verifiable();
            query.Setup(c => c.Dispose()).Verifiable();
        }

        [TestMethod]
        public void ReadQueryPassedDaoQuery()
        {
            var cmd = new Mock<IDaoQuery<IReadValue, IDbCommand>>(MockBehavior.Strict);
            cmd.Setup(c => c.ReadQuery(_read)).Returns(_strings).Verifiable();
            Assert.AreSame(_strings, Uut.ReadQuery(cmd.Object, _read));
            cmd.Verify();
        }

        [TestMethod]
        public void ReadQuery()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            SetUpReadQuery(uut.MockQuery);
            Assert.AreSame(_strings, uut.ReadQuery(sql, _setupQueryParameters, _read));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadQueryRowAndParams()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            SetUpReadRowAndParamsQuery(uut.MockQuery);
            Assert.AreSame(_strings, uut.ReadQuery(sql, _setupQueryParameters, _readAndParams));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadQueryNullSetUpParams()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            SetUpReadQuery(uut.MockQuery);
            Assert.AreSame(_strings, uut.ReadQuery(sql, _read));
            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();
        }

        private readonly Dictionary<string, string> _values = new Dictionary<string, string>()
        {
            ["One"] = "First",
            ["Two"] = "Second",
            ["Three"] = "Third",
        };

        private readonly DaoReadRow<string, IReadValue> _readValue = helper => helper[1].AsAnsiString;
        private readonly DaoReadRow<string, IReadValue> _readKey = helper => helper[1].AsAnsiString;

        private void SetUpReadQueryDict(Mock<IDaoQuery<IReadValue, IDbCommand>> query, bool willDispose)
        {
            query.Setup(c => c.ReadQuery(_readKey, _readValue)).Returns(_values).Verifiable();
            if (willDispose) query.Setup(c => c.Dispose()).Verifiable();
        }

        [TestMethod]
        public void ReadQueryDictPassedDaoQuery()
        {
            var cmd = new Mock<IDaoQuery<IReadValue, IDbCommand>>(MockBehavior.Strict);
            SetUpReadQueryDict(cmd, false);
            Assert.AreSame(_values, Uut.ReadQuery(cmd.Object, _readKey, _readValue));
            cmd.Verify();
        }

        [TestMethod]
        public void ReadQueryDict()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            SetUpReadQueryDict(uut.MockQuery, true);
            Assert.AreSame(_values, Uut.ReadQuery(sql, _setupQueryParameters, _readKey, _readValue));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadQueryDictNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            SetUpReadQueryDict(uut.MockQuery, true);
            Assert.AreSame(_values, Uut.ReadQuery(sql, _readKey, _readValue));
            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadQueryDictRowAndParams()
        {
            DaoReadRowAndParams<string, IReadValue> readValue = (helper, parameterHelper) => parameterHelper[1].AsString;
            DaoReadRowAndParams<string, IReadValue> readKey = (helper, parameterHelper) => parameterHelper[1].AsString;
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            uut.MockQuery.Setup(c => c.ReadQuery(readKey, readValue)).Returns(_values).Verifiable();
            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();
            Assert.AreSame(_values, Uut.ReadQuery(sql, _setupQueryParameters, readKey, readValue));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        private DaoReadRowAndParams<T, IReadValue> SetUpReadRowAndParamsQuery<T>(Mock<IDaoQuery<IReadValue, IDbCommand>> query, T value, T defaultValue)
        {
            DaoReadRowAndParams<T, IReadValue> read = (helper, parameterHelper) => defaultValue;
            query.Setup(c => c.ReadSingleRow(read, defaultValue)).Returns(value).Verifiable();
            query.Setup(c => c.Dispose()).Verifiable();
            return read;
        }

        private DaoReadRow<T, IReadValue> SetUpReadRowQuery<T>(Mock<IDaoQuery<IReadValue, IDbCommand>> query, T value, T defaultValue)
        {
            DaoReadRow<T, IReadValue> read = helper => defaultValue;
            query.Setup(c => c.ReadSingleRow(read, defaultValue)).Returns(value).Verifiable();
            query.Setup(c => c.Dispose()).Verifiable();
            return read;
        }

        [TestMethod]
        public void ReadSingleRowAndParams()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            var read = SetUpReadRowAndParamsQuery(uut.MockQuery, 100, 999);
            Assert.AreEqual(100, Uut.ReadSingleRow(sql, _setupQueryParameters, read, 999));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadSingleRow()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            var read = SetUpReadRowQuery(uut.MockQuery, 100, 999);
            Assert.AreEqual(100, Uut.ReadSingleRow(sql, _setupQueryParameters, read, 999));
            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadSingleRowNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            var read = SetUpReadRowQuery(uut.MockQuery, 100, 999);
            Assert.AreEqual(100, Uut.ReadSingleRow(sql, read, 999));
            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();
        }

        [TestMethod]
        public void ReadSingleString()
        {
            const string sql = "Some SQL To Test";
            const string result = "The Result";
            const string defaultValue = "A Default Value";
            var uut = OpenUut();
            DaoReadRow<string, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<string, IReadValue>>(), It.IsAny<string>()))
                .Returns(result)
                .Callback < DaoReadRow<string, IReadValue>, string>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleString(sql, _setupQueryParameters, defaultValue));

            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock< IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsString).Returns("A String Value").Verifiable();
            Assert.AreEqual("A String Value", read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleStringNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            const string result = "The Result";
            const string defaultValue = "A Default Value";
            var uut = OpenUut();
            DaoReadRow<string, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<string, IReadValue>>(), It.IsAny<string>()))
                .Returns(result)
                .Callback<DaoReadRow<string, IReadValue>, string>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleString(sql, defaultValue));

            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsString).Returns("A String Value").Verifiable();
            Assert.AreEqual("A String Value", read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleIntNullable()
        {
            const string sql = "Some SQL To Test";
            int? result = 1002;
            var uut = OpenUut();
            DaoReadRow<int?, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<int?, IReadValue>>(), It.IsAny<int?>()))
                .Returns(result)
                .Callback<DaoReadRow<int?, IReadValue>, string>((r, d) =>
                {
                    read = r;
                    Assert.IsNull(d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleIntNullable(sql, _setupQueryParameters));

            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsIntNullable).Returns(9910).Verifiable();
            Assert.AreEqual(9910, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleIntNullableNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            int? result = 1002;
            var uut = OpenUut();
            DaoReadRow<int?, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<int?, IReadValue>>(), It.IsAny<int?>()))
                .Returns(result)
                .Callback<DaoReadRow<int?, IReadValue>, string>((r, d) =>
                {
                    read = r;
                    Assert.IsNull(d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleIntNullable(sql));

            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsIntNullable).Returns(9910).Verifiable();
            Assert.AreEqual(9910, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleInt()
        {
            const string sql = "Some SQL To Test";
            const int result = 1002;
            const int defaultValue = 99;
            var uut = OpenUut();
            DaoReadRow<int, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<int, IReadValue>>(), It.IsAny<int>()))
                .Returns(result)
                .Callback<DaoReadRow<int, IReadValue>, int>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleInt(sql, _setupQueryParameters, defaultValue));

            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsInt).Returns(12312).Verifiable();
            Assert.AreEqual(12312, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleIntNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            const int result = 1002;
            const int defaultValue = 99;
            var uut = OpenUut();
            DaoReadRow<int, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<int, IReadValue>>(), It.IsAny<int>()))
                .Returns(result)
                .Callback<DaoReadRow<int, IReadValue>, int>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleInt(sql, defaultValue));

            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsInt).Returns(12312).Verifiable();
            Assert.AreEqual(12312, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleDate()
        {
            const string sql = "Some SQL To Test";
            DateTime result = new DateTime(2001,01,01);
            DateTime defaultValue =  new DateTime(2003, 03, 03);
            var uut = OpenUut();
            DaoReadRow<DateTime, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<DateTime, IReadValue>>(), It.IsAny<DateTime>()))
                .Returns(result)
                .Callback<DaoReadRow<DateTime, IReadValue>, DateTime>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleDate(sql, _setupQueryParameters, defaultValue));

            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsDate).Returns(defaultValue).Verifiable();
            Assert.AreEqual(defaultValue, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleDateNoParamsSetup()
        {
            const string sql = "Some SQL To Test";

            DateTime result = new DateTime(2001, 01, 01);
            DateTime defaultValue = new DateTime(2003, 03, 03);
            var uut = OpenUut();
            DaoReadRow<DateTime, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<DateTime, IReadValue>>(), It.IsAny<DateTime>()))
                .Returns(result)
                .Callback<DaoReadRow<DateTime, IReadValue>, DateTime>((r, d) =>
                {
                    read = r;
                    Assert.AreEqual(defaultValue, d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleDate(sql, defaultValue));

            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            readValue.Setup(i => i.AsDate).Returns(defaultValue).Verifiable();
            Assert.AreEqual(defaultValue, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }
        [TestMethod]
        public void ReadSingleDateNullable()
        {
            const string sql = "Some SQL To Test";
            DateTime result = new DateTime(2001, 01, 01);
            var uut = OpenUut();
            DaoReadRow<DateTime?, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<DateTime?, IReadValue>>(), It.IsAny<DateTime?>()))
                .Returns(result)
                .Callback<DaoReadRow<DateTime?, IReadValue>, DateTime?>((r, d) =>
                {
                    read = r;
                    Assert.IsNull(d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleDateNullable(sql, _setupQueryParameters));

            VerifyQuerySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            DateTime expected = new DateTime(2003, 03, 03);
            readValue.Setup(i => i.AsDateNullable).Returns(expected).Verifiable();
            Assert.AreEqual(expected, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ReadSingleDateNullableNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            DateTime result = new DateTime(2001, 01, 01);
            var uut = OpenUut();
            DaoReadRow<DateTime?, IReadValue> read = null;
            uut.MockQuery.Setup(c => c.ReadSingleRow(It.IsAny<DaoReadRow<DateTime?, IReadValue>>(), It.IsAny<DateTime?>()))
                .Returns(result)
                .Callback<DaoReadRow<DateTime?, IReadValue>, DateTime?>((r, d) =>
                {
                    read = r;
                    Assert.IsNull(d);
                })
                .Verifiable();

            uut.MockQuery.Setup(c => c.Dispose()).Verifiable();

            Assert.AreEqual(result, Uut.ReadSingleDateNullable(sql));

            VerifyQueryEmptySetupParams(sql);
            uut.MockQuery.Verify();

            Assert.IsNotNull(read);
            Mock<IReadHelper<IReadValue>> readValues = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
            Mock<IReadValue> readValue = new Mock<IReadValue>(MockBehavior.Strict);
            readValues.Setup(i => i[0]).Returns(readValue.Object).Verifiable();
            DateTime expected = new DateTime(2003, 03, 03);
            readValue.Setup(i => i.AsDateNullable).Returns(expected).Verifiable();
            Assert.AreEqual(expected, read(readValues.Object));
            readValues.Verify();
            readValue.Verify();
        }

        [TestMethod]
        public void ExecuteNonQueryIDaoCommand()
        {
            Mock<IDaoCommand<IReadValue,IDbCommand>> mock = new Mock<IDaoCommand<IReadValue, IDbCommand>>(MockBehavior.Strict);
            var result = new int[] {1, 2, 3, 4};
            mock.Setup(c => c.ExecuteNonQuery()).Returns(result).Verifiable();
            CollectionAssert.AreEqual(result, Uut.ExecuteNonQuery(mock.Object));
        }

        [TestMethod]
        public void ExecuteNonQueryNoReturns()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            var result = new int[] { 1, 2, 3, 4 };
            uut.MockCommand.Setup(c => c.ExecuteNonQuery()).Returns(result);
            uut.MockCommand.Setup(c => c.Dispose()).Verifiable();
            CollectionAssert.AreEqual(result, uut.ExecuteNonQuery(sql, _setupCommandParameters));
            VerifyCommandSetupParams(sql);
            uut.MockCommand.Verify();
        }

        [TestMethod]
        public void ExecuteNonQueryNoReturnsNoParamsSetup()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            var result = new int[] { 1, 2, 3, 4 };
            uut.MockCommand.Setup(c => c.ExecuteNonQuery()).Returns(result);
            uut.MockCommand.Setup(c => c.Dispose()).Verifiable();
            CollectionAssert.AreEqual(result, uut.ExecuteNonQuery(sql));
            VerifyCommandEmptySetupParams(sql);
            uut.MockCommand.Verify();
        }

        [TestMethod]
        public void ExecuteNonQuery()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            DaoOnExecute<string, IReadValue> onExecute = (helper, affected) => helper[0].AsString;
            uut.MockCommand.Setup(c => c.ExecuteNonQuery(onExecute)).Returns(_strings);
            uut.MockCommand.Setup(c => c.Dispose()).Verifiable();
            CollectionAssert.AreEqual(_strings, uut.ExecuteNonQuery(sql, _setupCommandParameters, onExecute));
            VerifyCommandSetupParams(sql);
            uut.MockCommand.Verify();
        }

        [TestMethod]
        public void ExecuteNonQueryNoParamSetup()
        {
            const string sql = "Some SQL To Test";
            var uut = OpenUut();
            DaoOnExecute<string, IReadValue> onExecute = (helper, affected) => helper[0].AsString;
            uut.MockCommand.Setup(c => c.ExecuteNonQuery(onExecute)).Returns(_strings);
            uut.MockCommand.Setup(c => c.Dispose()).Verifiable();
            CollectionAssert.AreEqual(_strings, uut.ExecuteNonQuery(sql, onExecute));
            VerifyCommandEmptySetupParams(sql);
            uut.MockCommand.Verify();
        }

        [TestMethod]
        public void ExecuteNonQueryIDaoCommandReturns()
        {
            Mock<IDaoCommand<IReadValue, IDbCommand>> mock = new Mock<IDaoCommand<IReadValue, IDbCommand>>(MockBehavior.Strict);
            DaoOnExecute<string, IReadValue> onExecute = (helper, affected) => helper[0].AsString;
            mock.Setup(c => c.ExecuteNonQuery(onExecute)).Returns(_strings).Verifiable();
            CollectionAssert.AreEqual(_strings, Uut.ExecuteNonQuery(mock.Object, onExecute));
        }
    }
}