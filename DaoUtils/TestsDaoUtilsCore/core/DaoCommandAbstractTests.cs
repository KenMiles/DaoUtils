using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DaoUtilsCore.log;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using DaoUtilsCore.def;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestsDaoUtilsCore.testHelpers;

namespace TestsDaoUtilsCore.core
{
    internal class TestableDaoCommandAbstract : DaoCommandAbstract<
                     IDaoParametersBuilderInput,
                     IDaoParametersBuilderInputOutput,
                     IDaoParametersBuilderOutput,
                     IReadValue,
                     IDbCommand
               >
    {
        public TestableDaoCommandAbstract(IDbCommand command, IDaoConnectionInfo connectionInfo, IDaoCommandParameterHelper paramHelper, ILog log)
            : base(command, connectionInfo, paramHelper, log)
        {
            ParamBuilder.Setup(s => s.Input).Returns(InputParamBuilder.Object);
        }


        public readonly Mock<IDaoParametersBuilderDirection<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>>
            ParamBuilder = new Mock<IDaoParametersBuilderDirection<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>>(MockBehavior.Strict);
        public Mock<IDaoParametersBuilderInput> InputParamBuilder = new Mock<IDaoParametersBuilderInput>(MockBehavior.Strict);

        public readonly List<string> ParamNamesPassed = new List<string>();
        public override IDaoParametersBuilderDirection<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput> Name(string parameterName)
        {
            ParamNamesPassed.Add(parameterName);
            return ParamBuilder.Object;
        }

        public Mock<IReadHelper<IReadValue>> MockReadHelper = new Mock<IReadHelper<IReadValue>>(MockBehavior.Strict);
        protected override IReadHelper<IReadValue> ReadHelper(List<IDaoParameterInternal> parameters)
        {
            Assert.AreSame(Parameters, parameters);
            return MockReadHelper.Object;
        }

        public Dictionary<IDataReader, Mock<IReadHelper<IReadValue>>> MockDataReadHelpers = new Dictionary<IDataReader, Mock<IReadHelper<IReadValue>>>();
        protected override IReadHelper<IReadValue> ReadHelper(IDataReader dataReader)
        {
            var result = new Mock<IReadHelper<IReadValue>>();
            MockDataReadHelpers[dataReader] = result;
            return result.Object;
        }

        public string AccessRemoveCommentsAndStringsFrom(string sql)
        {
            return RemoveCommentsAndStringsFrom(sql);
        }

        public string[] AccessSqlParameterNames(string sql)
        {
            return SqlParameterNames(sql);
        }

        public void AccessAttachParameters()
        {
            AttachParameters();
        }

        public void AddParameters(IEnumerable<IDaoParameterInternal> paramters)
        {
            Parameters.AddRange(paramters);
        }
    }

    class MockParams
    {
        public MockParams(
            int noParams,
            Mock<IDaoCommandParameterHelper> helper,
            Mock<IDataParameterCollection> paramters,
            Mock<IDaoConnectionInfo> connectionInfo,
            Mock<IDbCommand> command)
        {
            var mockParams = Enumerable
                .Range(1, noParams)
                .Select(i => new
                {
                    name = $"param_{i}",
                    mock = new Mock<IDaoParameterInternal>(MockBehavior.Strict),
                    paramMock = new Mock<IDbDataParameter>()
                })
                .ToArray();
            Array.ForEach(mockParams, m =>
            {
                m.paramMock.SetupSet(p => p.ParameterName = m.name).Verifiable();
                m.mock.Setup(p => p.Parameter).Returns(m.paramMock.Object).Verifiable();
                paramters.Setup(p => p.Add(m.paramMock.Object)).Returns(1).Verifiable();
            });
            helper.Setup(s => s.ParamertersByName())
                .Returns(mockParams.ToDictionary(i => i.name, i => i.mock.Object))
                .Verifiable();
            var paramNames = mockParams.Select(i => i.name).ToArray();
            var cmdText = string.Join(" ", paramNames.Select(s => $":{s}"));
            helper.Setup(c => c.ValidateParameters(It.IsAny<IEnumerable<string>>()))
                .Callback<IEnumerable<string>>(c => { Assert.AreEqual(string.Join("|", paramNames), string.Join("|", c)); })
                .Verifiable();
            paramters.Setup(c => c.Clear()).Verifiable();
            connectionInfo.Setup(c => c.ParamPrefix).Returns(":").Verifiable();
            command.Setup(c => c.CommandText).Returns(cmdText).Verifiable();
            _daoParams = mockParams.Select(c => c.mock).ToList();
            _params = mockParams.Select(c => c.paramMock).ToList();
        }

        private readonly List<Mock<IDaoParameterInternal>> _daoParams;
        private readonly List<Mock<IDbDataParameter>> _params;

        public void Verify()
        {
            _daoParams.ForEach(c => c.Verify());
            _params.ForEach(c => c.Verify());
        }
    }

    [TestClass]
    public class DaoCommandAbstractTests
    {

        private readonly Mock<IDbCommand> _command = new Mock<IDbCommand>(MockBehavior.Strict);
        private readonly Mock<IDataParameterCollection> _paramters = new Mock<IDataParameterCollection>(MockBehavior.Strict);
        private readonly Mock<IDaoConnectionInfo> _connectionInfo = new Mock<IDaoConnectionInfo>(MockBehavior.Strict);
        private readonly Mock<ILog> _log = new Mock<ILog>(MockBehavior.Loose);
        private readonly Mock<IDaoCommandParameterHelper> _helper = new Mock<IDaoCommandParameterHelper>(MockBehavior.Strict);

        private TestableDaoCommandAbstract _testing;

        [TestInitialize]
        public void SetUp()
        {
            _testing = new TestableDaoCommandAbstract(_command.Object, _connectionInfo.Object, _helper.Object, _log.Object);
            _command.Setup(p => p.Parameters).Returns(_paramters.Object);
            _log.Setup(l => l.Error(It.IsAny<object>(), It.IsAny<Exception>())).Callback<object, Exception>(
                (m, e) => Record(e, m?.ToString()));
            _log.Setup(l => l.Error(It.IsAny<object>())).Callback<object>(m => Record(m?.ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullCommand()
        {
            var test = new TestableDaoCommandAbstract(null, _connectionInfo.Object, _helper.Object, _log.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullConnectionInfo()
        {
            var test = new TestableDaoCommandAbstract(_command.Object, null, _helper.Object, _log.Object);
        }

        [TestMethod]
        public void NullLogAndHelperNoExcpetion()
        {
            var test = new TestableDaoCommandAbstract(_command.Object, _connectionInfo.Object, null, null);
        }

        [TestMethod]
        public void CommandSet()
        {
            Assert.AreSame(_command.Object, _testing.Command);
        }

        [TestMethod]
        public void Dispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            Assert.IsNull(_testing.Command);
            _command.Verify();
            _command.Setup(c => c.Dispose()).Throws(new Exception("Shouldn't be here"));
            _testing.Dispose();
            Assert.IsNull(_testing.Command);
        }

        [TestMethod]
        public void RemoveCommentsAndStringsFrom()
        {
            Assert.AreEqual("Outside   comments", _testing.AccessRemoveCommentsAndStringsFrom("Outside /* inside */ comments"));
            Assert.AreEqual("Outside   comments", _testing.AccessRemoveCommentsAndStringsFrom("Outside /* inside \r\n multi line \r\n block */ comments"));
            Assert.AreEqual("/Outside   *comments", _testing.AccessRemoveCommentsAndStringsFrom("/Outside /* *inside \r\n multi * line \r\n block */ *comments"));
            Assert.AreEqual("broken /* comments", _testing.AccessRemoveCommentsAndStringsFrom("broken /* comments"));
            Assert.AreEqual("broken */ comments", _testing.AccessRemoveCommentsAndStringsFrom("broken */ comments"));

            Assert.AreEqual(" ", _testing.AccessRemoveCommentsAndStringsFrom("/* inside \r\n multi line \r\n block */"));
            Assert.AreEqual("before ", _testing.AccessRemoveCommentsAndStringsFrom("before--commentsToEndOfText"));
            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before --comments to end of text"));
            Assert.AreEqual("before \nline after", _testing.AccessRemoveCommentsAndStringsFrom("before--comments\r\nline after"));
            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before -- :commetedOutParam"));
            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before -- @commetedOutParam"));
            Assert.AreEqual("before - -broken comments", _testing.AccessRemoveCommentsAndStringsFrom("before - -broken comments"));

            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before 'string'"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("'string' after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("'\"\\@/:string' after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("'`string' after"));
            Assert.AreEqual("'broken \nstring' after", _testing.AccessRemoveCommentsAndStringsFrom("'broken \nstring' after"));
            Assert.AreEqual("'broken \nstring", _testing.AccessRemoveCommentsAndStringsFrom("'broken \nstring"));
            Assert.AreEqual("    after   ", _testing.AccessRemoveCommentsAndStringsFrom("'string'' ''' after ''''"));

            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before \"string\""));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("\"string\" after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("\"'\\`/:string\" after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("\"~string\" after"));
            Assert.AreEqual("\"broken \nstring\" after", _testing.AccessRemoveCommentsAndStringsFrom("\"broken \nstring\" after"));
            Assert.AreEqual("\"broken \nstring", _testing.AccessRemoveCommentsAndStringsFrom("\"broken \nstring"));
            Assert.AreEqual("    after   ", _testing.AccessRemoveCommentsAndStringsFrom("\"string\"\" \"\"\" after \"\"\"\""));

            Assert.AreEqual("before  ", _testing.AccessRemoveCommentsAndStringsFrom("before `string`"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("`string` after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("`\"\\'/:string` after"));
            Assert.AreEqual("  after", _testing.AccessRemoveCommentsAndStringsFrom("`~string` after"));
            Assert.AreEqual("`broken \nstring` after", _testing.AccessRemoveCommentsAndStringsFrom("`broken \nstring` after"));
            Assert.AreEqual("`broken \nstring", _testing.AccessRemoveCommentsAndStringsFrom("`broken \nstring"));
            Assert.AreEqual("    after   ", _testing.AccessRemoveCommentsAndStringsFrom("`string`` ``` after ````"));
        }

        private void CheckSqlParameterNames(string sql, params string[] paramNames)
        {
            Assert.AreEqual(string.Join("|", paramNames), string.Join("|", _testing.AccessSqlParameterNames(sql)));
        }

        [TestMethod]
        public void SqlParameterNames()
        {
            _connectionInfo.Setup(c => c.ParamPrefix).Returns("~").Verifiable();
            CheckSqlParameterNames("~paramOne", "paramOne");
            CheckSqlParameterNames("~paramOne '~paramTwo'", "paramOne");
            CheckSqlParameterNames("~paramOne -- ~paramTwo \r\n ~paramThree paramFour", "paramOne", "paramThree");
            CheckSqlParameterNames("'~paramTwo' ~paramOne", "paramOne");
            CheckSqlParameterNames("\"~paramTwo\" \r~paramOne", "paramOne");
            CheckSqlParameterNames("~paramOne \r ~paramTwo ~paramThree", "paramOne", "paramTwo", "paramThree");
            CheckSqlParameterNames("~paramOne ~ paramTwo ~paramThree", "paramOne", "paramThree");

            _connectionInfo.Verify();
        }

        private MockParams MockParams(int noParams)
        {
            return new MockParams(noParams, _helper, _paramters, _connectionInfo, _command);
        }

        [TestMethod]
        public void AttachParameters()
        {
            var mockParams = MockParams(5);

            _testing.AccessAttachParameters();

            _paramters.Verify();
            _command.Verify();

            mockParams.Verify();
        }

        [TestMethod]
        public void SetUpParamsForQuery()
        {
            MockParams mockParams = null;
            DaoSetupParameters<IDaoParametersBuilderInput> setupParameters = c => { mockParams = MockParams(2); };
            Assert.AreSame(_testing, _testing.SetupParameters(setupParameters));
            Assert.IsNotNull(mockParams);
            mockParams.Verify();
            _paramters.Verify();
            _command.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void SetUpParamsForQueryAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            DaoSetupParameters<IDaoParametersBuilderInput> setupParameters = c => { MockParams(2); };
            _testing.SetupParameters(setupParameters);
        }

        [TestMethod]
        public void SetUpParamsForProc()
        {
            MockParams mockParams = null;
            DaoSetupParameters<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>
                setupParameters = c => { mockParams = MockParams(2); };
            Assert.AreSame(_testing, _testing.SetupParameters(setupParameters));
            Assert.IsNotNull(mockParams);
            mockParams.Verify();
            _paramters.Verify();
            _command.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void SetUpParamsForProcAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            DaoSetupParameters<IDaoParametersBuilderInput, IDaoParametersBuilderInputOutput, IDaoParametersBuilderOutput>
                setupParameters = c => { MockParams(2); };
            _testing.SetupParameters(setupParameters);
        }

        [TestMethod]
        public void NamedInputParamBuilder()
        {
            IDaoSetupParametersHelper<IDaoParametersBuilderInput> testing = _testing;
            Assert.AreSame(_testing.InputParamBuilder.Object, testing.Name("A Parameter"));
            Assert.AreEqual("A Parameter", string.Join("|", _testing.ParamNamesPassed));
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ExecuteNonQueryAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ExecuteNonQuery();
        }

        private readonly List<Exception> _exceptions = new List<Exception>();
        readonly List<string> _calls = new List<string>();

        private void Record(string call)
        {
            _calls.Add(call);
        }

        private void Record(Exception exception, string message, string call = "Error")
        {
            Record($"Log.{call}({message}, {exception?.GetType().Name})");
            _exceptions.Add(exception);
        }

        private void SetupHelperExecute(CommandExecuteMode mode, int noCalls)
        {
            _helper.Setup(h => h.Execute(mode, It.IsAny<CommandActionDelegate>()))
                .Callback<CommandExecuteMode, CommandActionDelegate>(
                    (m, d) =>
                    {
                        Record($"HelperExecute({mode}, delegate)");
                        for (int idx = 0; idx < noCalls; idx++)
                        {
                            d(idx);
                        }
                    });
        }

        private void SetupNonQuery(int noCalls, int queryReturn = 1, string query = "The Query")
        {
            var trans = new Mock<IDbTransaction>(MockBehavior.Strict);

            // if excpetion thrown then will call CommandText - but since likely will be a moq exception don't want to lose it
            // So don't lose
            _command.Setup(c => c.CommandText).Returns(query);
            _connectionInfo.Setup(c => c.ActiveTransaction).Returns(trans.Object).Verifiable();

            _connectionInfo.Setup(c => c.WaitOpen()).Callback(() => Record("WaitOpen"));
            _command.SetupSet(c => c.Transaction = trans.Object).Callback(() => Record("Set Transaction"));

            _command.Setup(c => c.ExecuteNonQuery()).Returns(queryReturn).Callback(() => Record("ExecuteNonQuery"));
            _command.SetupSet(c => c.Transaction = null).Callback(() => Record(ClearTransaction));
            SetupHelperExecute(CommandExecuteMode.NonQuery, noCalls);
        }

        private readonly string[] _queryStartCalls = { "WaitOpen", "Set Transaction", "HelperExecute(Query, delegate)" };

        private readonly string[] _nonQueryStartCalls =
        {
            "WaitOpen", "Set Transaction",
            "HelperExecute(NonQuery, delegate)"
        };

        private const string ClearTransaction = "Clear Transaction";
        private readonly string[] _endCalls = { ClearTransaction };

        private List<string> NonQueryExpected(int noCalls)
        {
            return new List<string>(_nonQueryStartCalls)
                .Concat(Enumerable.Range(0, noCalls).Select(s => "ExecuteNonQuery"))
                .Concat(_endCalls)
                .ToList();
        }

        private void VerifyCalls(params IEnumerable<string>[] expectedCallBatches)
        {
            var expectedCalls = expectedCallBatches.Select(c => c.ToArray()).SelectMany(c => c);
            Assert.AreEqual(string.Join("\n", expectedCalls), string.Join("\n", _calls));
        }

        [TestMethod]
        public void ExecuteNonQuery()
        {
            SetupNonQuery(6, 9876);
            var result = _testing.ExecuteNonQuery();
            Assert.AreEqual("9876,9876,9876,9876,9876,9876", string.Join(",", result.Select(i => $"{i}")));
            VerifyCalls(NonQueryExpected(6));
        }

        [TestMethod]
        public void ExecuteNonQueryLogsException()
        {
            SetupNonQuery(12, 1, "The Query Executed");
            var e = new Exception("The Exception Message");
            _command.Setup(c => c.ExecuteNonQuery()).Throws(e);
            try
            {
                _testing.ExecuteNonQuery();
                Assert.IsFalse(true, "Shouldn't be here");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(e, ex);
            }
            CollectionAssert.AreEqual(new[] { e }, _exceptions);
            VerifyCalls(_nonQueryStartCalls,
                new [] {
                    "Log.Error(\"Exception\" - \"The Exception Message\" thrown during executing non query \"The Query Executed\", Exception)",
                    ClearTransaction
                } 
            );
        }

        private List<Mock<IDaoParameterInternal>> MockOutputParams(params int[] outputParamArraySize)
        {
            var result = outputParamArraySize.Select((s, i) =>
            {
                var p = new Mock<IDaoParameterInternal>(MockBehavior.Strict);
                p.Setup(ps => ps.OutputParamArraySize).Returns(s).Callback(() => Record($"OutputParamArraySize:{i}"));
                return p;
            }).ToList();
            _testing.AddParameters(result.Select(s => s.Object));
            return result;
        }

        [TestMethod]
        public void ExecuteNonQueryReturnsValues()
        {
            MockOutputParams(0, 5, 5, 0, 5);
            SetupNonQuery(5, 3);
            _helper.Setup(h => h.ReadReturnedParams(It.IsAny<CommandActionDelegate>()))
                .Callback<CommandActionDelegate>(d =>
                {
                    Record($"ReadReturnedParams");
                    for (var idx = 0; idx < 5; idx++)
                    {
                        d(idx);
                    }
                });
            var result = _testing.ExecuteNonQuery(((helper, affected) =>
            {
                Record("OnExecute");
                return new { helper, affected };
            }));
            Assert.AreEqual("3,3,3,3,3", string.Join(",", result.Select(r => r.affected)));
            Assert.AreEqual(1, result.Select(r => r.helper).Distinct().Count());
            Assert.AreSame(_testing.MockReadHelper.Object, result.Select(s => s.helper).FirstOrDefault());
            VerifyCalls(NonQueryExpected(5),
                new[] { "ReadReturnedParams",
                        "OnExecute",
                        "OnExecute",
                        "OnExecute",
                        "OnExecute",
                        "OnExecute"});
        }


        private readonly List<IDataReader> _readers = new List<IDataReader>();
        private void MockDataReader(int call, int noCalls, params int[] noRows)
        {
            if (call >= noCalls)
            {
                _command.Setup(c => c.ExecuteReader()).Throws(new Exception($"Shouldn't be calling this - call {call}"));
                return;
            }
            var read = new Mock<IDataReader>(MockBehavior.Strict);
            _command.Setup(c => c.ExecuteReader())
                .Returns(read.Object)
                .Callback(() => Record($"ExecuteReader:{call}"));
            var idx = call >= noRows.Length ? 2 : noRows[call];
            read.Setup(c => c.Read())
                .Returns(() => idx-- > 0)
                .Callback(() =>
                {
                    Record($"dr_{call}.Read");
                    if (idx == 0) MockDataReader(call + 1, noCalls, noRows);
                });
            read.Setup(c => c.Dispose()).Callback(() => Record($"dr_{call}.Dispose"));
            _readers.Add(read.Object);
        }

        private void SetupQuery(int noCalls, string query = "The Query", params int[] rows)
        {
            var trans = new Mock<IDbTransaction>(MockBehavior.Strict);

            // if excpetion thrown then will call CommandText - but since likely will be a moq exception don't want to lose it
            // So don't lose
            _command.Setup(c => c.CommandText).Returns(query);
            _connectionInfo.Setup(c => c.ActiveTransaction).Returns(trans.Object).Verifiable();

            _connectionInfo.Setup(c => c.WaitOpen()).Callback(() => Record("WaitOpen"));
            _command.SetupSet(c => c.Transaction = trans.Object).Callback(() => Record("Set Transaction"));

            MockDataReader(0, noCalls, rows);
            _command.SetupSet(c => c.Transaction = null).Callback(() => Record(ClearTransaction));
            SetupHelperExecute(CommandExecuteMode.Query, noCalls);
            _helper.Setup(h => h.RecordRow()).Callback(() => Record("RecordRow"));
        }

        private List<string> QueryCall(string[] onReadRow, int idx, int noRows, bool includeLast = true)
        {
            var rowCalls = Enumerable.Range(0, noRows)
                .Select(s => new[] { $"dr_{idx}.Read" }.Concat(onReadRow).ToArray())
                .SelectMany(l => l);
            var calls = new[] { $"ExecuteReader:{idx}" }
                 .Concat(rowCalls)
                 .ToList();
            if (includeLast) calls.Add($"dr_{idx}.Read");
            calls.Add($"dr_{idx}.Dispose");
            return calls;
        }

        private List<string> QueryCalls(string[] onReadRow, params int[] noRows)
        {
            return noRows
                .Select((c, i) => QueryCall(onReadRow, i, c))
                .SelectMany(c => c)
                .ToList();
        }

        private List<string> QueryCalls(string[] onReadRow, int noRows, bool includeLast)
        {
            return QueryCall(onReadRow, 0, noRows, includeLast).ToList();
        }

        [TestMethod]
        public void ReadQueryDataReaderAndParams()
        {
            var allParams = MockOutputParams(0, 3, 3, 0, 3);
            SetupQuery(3, "The Query", 3, 3, 3);
            var result = _testing.ReadQuery((helper, parameterHelper) =>
            {
                Record("ReadRow");
                return new { helper, parameterHelper };
            });

            VerifyCalls(_queryStartCalls, QueryCalls(new[] { "RecordRow", "ReadRow" }, 3, 3, 3), _endCalls);
            Assert.AreEqual(1, result.Select(r => r.parameterHelper).Distinct().Count());
            Assert.AreSame(_testing.MockReadHelper.Object, result.Select(s => s.parameterHelper).FirstOrDefault());

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelpers = _testing.MockDataReadHelpers.Values.Select(s => s.Object).ToArray();
            var resultHelpers = result.Select(s => s.helper).Distinct().ToArray();
            CollectionAssert.AreEqual(expectedHelpers, resultHelpers);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderAndParamtersNullReadRow()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((DaoReadRowAndParams<int, IReadValue>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadQueryDataReaderAndParamsAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((h, p) => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderNullReadRow()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((DaoReadRow<int, IReadValue>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadQueryDataReaderAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery(h => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadQueryDataReaderAndParamsToDictAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((h, p) => 1, (h, p) => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderAndParamtersToDictionaryNullReadRow()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((h, p) => 1, (DaoReadRowAndParams<int, IReadValue>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderAndParamtersToDictionaryNullRowKey()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((DaoReadRowAndParams<int, IReadValue>)null, (h, p) => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadQueryDataReaderToDictAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery(h => 1, h => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderToDictionaryNullReadRow()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery(h => 1, (DaoReadRow<int, IReadValue>)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReadQueryDataReaderToDictionaryNullRowKey()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadQuery((DaoReadRow<int, IReadValue>)null, h => 1);
        }

        [TestMethod]
        public void ReadQueryDataReaderAndParamsLogsException()
        {
            var allParams = MockOutputParams(0, 3, 3, 0, 3);
            SetupQuery(1, "A Query", 0);
            var ex = new Exception("An Exception Message");
            _command.Setup(c => c.ExecuteReader()).Throws(ex);

            try
            {
                var result = _testing.ReadQuery((helper, parameterHelper) =>
                {
                    Record("ReadRow");
                    return new { helper, parameterHelper };
                });
            }
            catch (Exception expThrown)
            {
                Assert.AreEqual(ex, expThrown);
            }
            CollectionAssert.AreEqual(new[] { ex }, _exceptions);
            VerifyCalls(_queryStartCalls, new[] { "Log.Error(\"Exception\" - \"An Exception Message\" thrown during Reading Query \"A Query\", Exception)" }, _endCalls);
        }

        [TestMethod]
        public void ReadQueryDataReader()
        {
            var allParams = MockOutputParams(0, 3, 3, 0, 3);
            SetupQuery(3, "The Query", 3, 3, 3);
            var result = _testing.ReadQuery(helper =>
            {
                Record("ReadRow");
                return helper;
            });

            VerifyCalls(_queryStartCalls, QueryCalls(new[] { "RecordRow", "ReadRow" }, 3, 3, 3), _endCalls);

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelpers = _testing.MockDataReadHelpers.Values.Select(s => s.Object).ToArray();
            var resultHelpers = result.Distinct().ToArray();
            CollectionAssert.AreEqual(expectedHelpers, resultHelpers);
        }

        [TestMethod]
        public void ReadQueryToDictionaryDataReaderAndParams()
        {
            var allParams = MockOutputParams(0, 0);
            SetupQuery(3, "A Query for Dictionary", 2, 3, 4);

            var keys = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var keyQueue = new Queue<int>(keys);
            var values = new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            var valueQueue = new Queue<string>(values);
            var helperToValues = new Dictionary<IReadHelper<IReadValue>, List<string>>();

            var result = _testing.ReadQuery((helper, parameterHelper) =>
                    {
                        var key = keyQueue.Dequeue();
                        Record("ReadKey");
                        Assert.AreEqual(parameterHelper, _testing.MockReadHelper.Object);
                        if (!helperToValues.ContainsKey(helper)) helperToValues[helper] = new List<string>();
                        helperToValues[helper].Add($"{key}");
                        return key;
                    }
                    , (helper, parameterHelper) =>
                    {
                        var value = valueQueue.Dequeue();
                        Record("ReadValue");
                        Assert.AreEqual(parameterHelper, _testing.MockReadHelper.Object);
                        if (!helperToValues.ContainsKey(helper)) helperToValues[helper] = new List<string>();
                        helperToValues[helper].Add(value);
                        return value;
                    }
                );

            CheckArrays.CheckEqual("Result Keys", result.Keys, keys.Select(i => $"{i}"));
            CheckArrays.CheckEqual("Result Values", result.Values, values);

            //Check the values were batched with right helper
            CheckArrays.CheckEqual("Helper and Values Returned", 
                helperToValues.Values.Select(v => string.Join(",", v)), 
                "1,One,2,Two", "3,Three,4,Four,5,Five", "6,Six,7,Seven,8,Eight,9,Nine");

            VerifyCalls(_queryStartCalls, QueryCalls(new[] { "RecordRow", "ReadKey", "ReadValue" }, 2, 3, 4), _endCalls);

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelpers = _testing.MockDataReadHelpers.Values.Select(s => s.Object).ToArray();
            CollectionAssert.AreEqual(expectedHelpers, helperToValues.Keys);
        }

        [TestMethod]
        public void ReadQueryToDictionaryDataReader()
        {
            var allParams = MockOutputParams(0, 0);
            SetupQuery(3, "A Query for Dictionary", 2, 3, 4);
            var keys = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var keyQueue = new Queue<int>(keys);
            var values = new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            var valueQueue = new Queue<string>(values);
            var helperToValues = new Dictionary<IReadHelper<IReadValue>, List<string>>();

            var result = _testing.ReadQuery(helper =>
            {
                var key = keyQueue.Dequeue();
                Record("ReadKey");
                if (!helperToValues.ContainsKey(helper)) helperToValues[helper] = new List<string>();
                helperToValues[helper].Add($"{key}");
                return key;
            }
                    , helper =>
                    {
                        var value = valueQueue.Dequeue();
                        Record("ReadValue");
                        if (!helperToValues.ContainsKey(helper)) helperToValues[helper] = new List<string>();
                        helperToValues[helper].Add(value);
                        return value;
                    }
                );

            CheckArrays.CheckEqual("Result Keys", result.Keys, keys.Select(i => $"{i}"));
            CheckArrays.CheckEqual("Result Values", result.Values, values);

            //Check the values were batched with right helper
            CheckArrays.CheckEqual("Helper and Values Returned",
                helperToValues.Values.Select(v => string.Join(",", v)),
                "1,One,2,Two", "3,Three,4,Four,5,Five", "6,Six,7,Seven,8,Eight,9,Nine");

            VerifyCalls(_queryStartCalls, QueryCalls(new[] { "RecordRow", "ReadKey", "ReadValue" }, 2, 3, 4), _endCalls);

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelpers = _testing.MockDataReadHelpers.Values.Select(s => s.Object).ToArray();
            CollectionAssert.AreEqual(expectedHelpers, helperToValues.Keys);
        }

        [TestMethod]
        public void ReadQueryToDictionaryDataReaderAndParamsLogsDuplicate()
        {
            var allParams = MockOutputParams(0, 0);
            SetupQuery(3, "A Query for Dictionary", 3);

            var keyQueue = new Queue<int>(new[] { 1, 2, 1 });
            var valueQueue = new Queue<string>(new[] { "One", "Two", "Three"});

            try
            {
                var result = _testing.ReadQuery((helper, parameterHelper) => keyQueue.Dequeue(),
                    (helper, parameterHelper) => valueQueue.Dequeue());
                Assert.IsFalse(true, "shouldn't be here");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreEqual("Error add/reading Key '1'", e.Message);
            }

            VerifyCalls(_queryStartCalls, 
                QueryCalls(new [] {"RecordRow"}, 3, false), 
                new[] { "Log.Error(\"DaoUtilsException\" - \"Error add/reading Key '1'\" thrown during Reading Query \"A Query for Dictionary\", DaoUtilsException)" }, 
                _endCalls);
        }

        [TestMethod]
        public void ReadQueryToDictionaryDataReaderAndParamsLogsException()
        {
            MockOutputParams(0, 0);
            SetupQuery(3, "A Query for Dictionary", 1);
            var ex = new Exception("An Exception Message");
            _command.Setup(c => c.ExecuteReader()).Throws(ex);

            try
            {
                var result = _testing.ReadQuery((helper, parameterHelper) => 1, (helper, parameterHelper) => "A");
                Assert.IsFalse(true, "shouldn't be here");
            }
            catch (Exception e)
            {
                Assert.AreEqual(ex, e);
            }

            VerifyCalls(_queryStartCalls,
                new[]
                {
                    "Log.Error(\"Exception\" - \"An Exception Message\" thrown during Reading Query \"A Query for Dictionary\", Exception)"
                },
                _endCalls);
        }

        [TestMethod]
        public void ReadSingleRowDataReaderAndParams()
        {
            var allParams = MockOutputParams(0, 1, 0, 0, 1);
            SetupQuery(1, "The Query", 1);

            var result = _testing.ReadSingleRow((helper, parameterHelper) =>
            {
                Record("ReadRow");
                return new { helper, parameterHelper };
            });

            VerifyCalls(_queryStartCalls, 
                new[] { "ExecuteReader:0", "dr_0.Read", "RecordRow", "ReadRow", "dr_0.Dispose" }
                , _endCalls);
            Assert.AreSame(_testing.MockReadHelper.Object, result.parameterHelper);

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelper = _testing.MockDataReadHelpers.Values.FirstOrDefault();
            Assert.AreEqual(expectedHelper?.Object, result.helper);
        }

        [TestMethod]
        public void ReadSingleRowDataReader()
        {
            var allParams = MockOutputParams(0, 1, 0, 0, 1);
            SetupQuery(1, "The Query", 1);
            var result = _testing.ReadSingleRow(helper =>
            {
                Record("ReadRow");
                return helper;
            });

            VerifyCalls(_queryStartCalls,
                new[] { "ExecuteReader:0", "dr_0.Read", "RecordRow", "ReadRow", "dr_0.Dispose" }
                , _endCalls);

            CollectionAssert.AreEqual(_readers, _testing.MockDataReadHelpers.Keys);
            var expectedHelper = _testing.MockDataReadHelpers.Values.FirstOrDefault();
            Assert.AreEqual(expectedHelper?.Object, result);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadSingleRowDataReaderAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadSingleRow(h => 1);
        }

        [TestMethod]
        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ReadSingleRowDataReaderAndParamsAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ReadSingleRow((h, p) => 1);
        }

        [TestMethod]
        public void ExecuteScalar()
        {
            _command.Setup(c => c.ExecuteScalar()).Returns("A Bit of Test data").Verifiable();
            Assert.AreEqual("A Bit of Test data", _testing.ExecuteScalar());
            _command.Verify();
        }

        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ExecuteScalarAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ExecuteScalar();
        }

        [ExpectedException(typeof(CommandDisposedOfException))]
        public void ExecuteScalarAndConvertAfterDispose()
        {
            _command.Setup(c => c.Dispose()).Verifiable();
            _testing.Dispose();
            _testing.ExecuteScalar<string>();
        }

        [TestMethod]
        public void ExecuteScalarLogsException()
        {
            var e = new Exception("A Test Exception");
            _command.Setup(c => c.CommandText).Returns("A Query For Scalar");
            _log.Setup(l => l.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<object, Exception>((msg, ex) => {
                    Assert.AreEqual("\"Exception\" - \"A Test Exception\" thrown during ExecuteScalar \"A Query For Scalar\"", msg);
                    Assert.AreSame(e, ex);
                })
                .Verifiable();
            _command.Setup(c => c.ExecuteScalar()).Throws(e);
            try
            {
                var test = _testing.ExecuteScalar();
                Assert.Fail("Shouldn't be here");
            }
            catch (Exception ex)
            {
                if (e != ex) throw;
            }
            _log.Verify();
        }

        [TestMethod]
        public void ExecuteScalarLogsConvertionException()
        {
            var expectedMessage = 
            _command.Setup(c => c.CommandText).Returns("A Query For Scalar");
            _log.Setup(l => l.Error(It.IsAny<object>(), It.IsAny<Exception>()))
                .Callback<object, Exception>((msg, ex) => {
                    Assert.AreEqual("\"FormatException\" - \"Input string was not in a correct format.\" thrown during converting to System.Int32 value \"Hello\" returned by ExecuteScalar \"A Query For Scalar\"", msg);
                    Assert.IsNotNull(ex);
                })
                .Verifiable();
            _command.Setup(c => c.ExecuteScalar()).Returns("Hello");
            try
            {
                var test = _testing.ExecuteScalar<int>();
                Assert.Fail("Shouldn't be here");
            }
            catch (FormatException ex)
            {
                Assert.AreEqual("Input string was not in a correct format.", ex.Message);
            }
            _log.Verify();
        }

        private void ExecuteScalarConverstion<T>(object value, T expected)
            where T: IConvertible
        {
            _command.Setup(c => c.ExecuteScalar()).Returns(value).Verifiable();
            Assert.AreEqual(expected, _testing.ExecuteScalar<T>());
            _command.Verify();
        }

        [TestMethod]
        public void ExecuteScalarConverstion()
        {
            ExecuteScalarConverstion("Hello", "Hello");
            ExecuteScalarConverstion(1, 1);
            ExecuteScalarConverstion<sbyte>(1, 1);
            ExecuteScalarConverstion(1.4, 1);
            ExecuteScalarConverstion(1.4, 1.4);
            ExecuteScalarConverstion("1.4", 1.4);
        }
    }
}
