using System;
using System.Collections;
using System.Collections.Generic;
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
    internal class AccessDaoCommandParameterHelper : DaoCommandParameterHelper
    {
        public AccessDaoCommandParameterHelper(List<IDaoParameterInternal> parameters, ILog log) : base(parameters, log)
        {
        }

        public AccessDaoCommandParameterHelper(List<IDaoParameterInternal> parameters) : base(parameters)
        {
        }

        private List<string> RecordCalls = new List<string>();

        public void RecordCall(string call)
        {
            RecordCalls.Add(call);
        }

        protected override void PreOnExecute(int noCallsRequired, bool isQuery)
        {
            //base.PreOnExecute(noCallsRequired, isQuery);
            RecordCall($"PreOnExecute({noCallsRequired}, {isQuery})");
        }

        public int AccessNoCallsRequired(bool queryMode)
        {
            return NoCallsRequired(queryMode);
        }

        public string[] Execute(CommandExecuteMode mode)
        {
            RecordCalls.Clear();
            Execute(mode, callIdx => RecordCall($"onExecute({callIdx})"));
            return RecordCalls.ToArray();
        }
    }

    [TestClass]
    public class DaoCommandParameterHelperTests
    {
        [TestMethod]
        public void NoOfCallsStr()
        {
            Assert.AreEqual("", DaoCommandParameterHelper.NoOfCallsStr(1, 0));
            Assert.AreEqual("0 of 88 calls", DaoCommandParameterHelper.NoOfCallsStr(0, 88));
            Assert.AreEqual("1 of 88 calls", DaoCommandParameterHelper.NoOfCallsStr(1, 88));
            Assert.AreEqual("", DaoCommandParameterHelper.NoOfCallsStr(1, 1));
            Assert.AreEqual("34 of 88 calls", DaoCommandParameterHelper.NoOfCallsStr(34, 88));
            Assert.AreEqual("row 1", DaoCommandParameterHelper.NoOfCallsStr(0, 0, 1));
            Assert.AreEqual("row 99", DaoCommandParameterHelper.NoOfCallsStr(0, 0, 99));
            Assert.AreEqual("row 99; 2 of 3 calls", DaoCommandParameterHelper.NoOfCallsStr(2, 3, 99));
        }

        private readonly Mock<ILog> _log = new Mock<ILog>();

        private AccessDaoCommandParameterHelper UnderTest(params Mock<IDaoParameterInternal>[] mockParams)
        {
            return new AccessDaoCommandParameterHelper(mockParams.Select(p => p.Object).ToList(), _log.Object);
        }

        private Mock<IDaoParameterInternal> Param()
        {
            return new Mock<IDaoParameterInternal>(MockBehavior.Strict);
        }

        private Mock<IDaoParameterInternal> Input(int inputArraySize, bool isOutputAsWell = false)
        {
            var input = Param();
            input.Setup(i => i.InputParamArraySize).Returns(inputArraySize);
            input.Setup(i => i.IsInput).Returns(true);
            input.Setup(i => i.IsOutput).Returns(isOutputAsWell);
            return input;
        }

        private Mock<IDaoParameterInternal> Output()
        {
            var output = Param();
            output.Setup(i => i.IsInput).Returns(false);
            output.Setup(i => i.IsOutput).Returns(true);
            output.Setup(o => o.InputParamArraySize).Returns(0);
            return output;
        }

        private void SetUpPreCall(int index, params Mock<IDaoParameterInternal>[] mockParams)
        {
            Array.ForEach(mockParams, m => m.Setup(p => p.PreCall(index)).Verifiable());
        }

        private void SetUpPostCall(params Mock<IDaoParameterInternal>[] mockParams)
        {
            Array.ForEach(mockParams, m => m.Setup(p => p.PostCall()).Verifiable());
        }

        private void SetUpSetReadIndex(int index, params Mock<IDaoParameterInternal>[] mockParams)
        {
            Array.ForEach(mockParams, m => m.Setup(p => p.SetReadIndex(index)).Verifiable());
        }

        private void SetUpForLog(params Mock<IDaoParameterInternal>[] mockParams)
        {
            var paramList = mockParams.Select((p, i) => new { param = p, idx = i }).ToList();
            paramList.ForEach(mp =>
            {
                mp.param
                  .Setup(p => p.ForLog(It.IsAny<bool>()))
                  .Returns<bool>(b => $"{b}-{mp.idx}");
            });
        }

        [TestMethod]
        public void PreCall()
        {
            var ip1 = Input(10, false);
            var ip2 = Input(20, true);
            SetUpPreCall(99, ip1, ip2);
            var uut = UnderTest(Input(0, false), ip1, Input(0, true), Output(), ip2, Output());

            uut.PreCall(99);

            ip1.Verify();
            ip2.Verify();

        }

        [TestMethod]
        public void PostCall()
        {
            var op1 = Output();
            var op2 = Input(20, true);
            SetUpPostCall(op1, op2);
            var uut = UnderTest(Input(0), op1, Input(10), Input(0), op2);

            uut.PostCall();

            op1.Verify();
            op2.Verify();
        }

        [TestMethod]
        public void SetReadParameterIndex()
        {
            var paramList = new[] { Output(), Input(10, false), Input(20, true), Input(0), Output() };
            var uut = UnderTest();

            SetUpSetReadIndex(10900, paramList.ToArray());
            uut.SetReadParameterIndex(paramList.Select(p => p.Object).ToList(), 10900);

        }

        private string ExoectedParamValues(bool readReturnValuesStage, int noParams)
        {
            var values = Enumerable.Range(0, noParams).Select(i => $"{readReturnValuesStage}-{i}");
            return string.Join("; ", values);
        }

        [TestMethod]
        public void ParamValues()
        {
            var paramList = new[] { Output(), Input(10, false), Input(20, true), Input(0), Output() };
            var uut = UnderTest(paramList);

            SetUpForLog(paramList);
            Assert.AreEqual(ExoectedParamValues(true, paramList.Length), uut.ParamValues(true));
            Assert.AreEqual(ExoectedParamValues(false, paramList.Length), uut.ParamValues(false));

        }
        private Mock<IDaoParameterInternal>[] Output(List<string> recordOn, params int[] outputParamArraySize)
        {
            return outputParamArraySize.Select((s, idx) =>
            {
                var output = Param();
                output.Setup(i => i.OutputParamArraySize).Returns(s);
                output.Setup(i => i.SetReadIndex(It.IsAny<int>())).Callback<int>(
                    c => recordOn.Add($"params[{idx}].SetReadIndex({c})"));
                return output;
            }).ToArray();
        }


        [TestMethod]
        public void ReadReturnedParams()
        {
            var recorded = new List<string>();
            var uut = UnderTest(Output(recorded, 0, 1, 2, 0, 0, 2));
            uut.ReadReturnedParams(i => { recorded.Add($"onRead({i})"); });
            string[] expected = {
                "params[1].SetReadIndex(0)",
                "params[2].SetReadIndex(0)",
                "params[5].SetReadIndex(0)",
                "onRead(0)",
                "params[1].SetReadIndex(1)",
                "params[2].SetReadIndex(1)",
                "params[5].SetReadIndex(1)",
                "onRead(1)"
            };
            Assert.AreEqual(string.Join("\r\n", expected), string.Join("\r\n", recorded));
        }

        [TestMethod]
        public void ReadReturnedParamsLogsExceptions()
        {
            var recorded = new List<string>();
            var mockParams = Output(recorded, 10);
            var uut = UnderTest(mockParams);
            var expectedMessage = "Testing Error on 4 of 10 calls, params = True-0";
            var exception = new Exception("Testing Error");
            _log.Setup(l => l.Error(expectedMessage, exception)).Verifiable();
            SetUpForLog(mockParams);
            try
            {
                uut.ReadReturnedParams(i =>
                {
                    if (i >= 4) throw exception;
                    recorded.Add($"onRead({i})");
                });
                Assert.Fail("No Exception Thrown");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreEqual(expectedMessage, e.Message);
                Assert.AreEqual(exception, e.InnerException);
            }
            string[] expected = {
                "params[0].SetReadIndex(0)",
                "onRead(0)",
                "params[0].SetReadIndex(1)",
                "onRead(1)",
                "params[0].SetReadIndex(2)",
                "onRead(2)",
                "params[0].SetReadIndex(3)",
                "onRead(3)",
                "params[0].SetReadIndex(4)",
            };
            Assert.AreEqual(string.Join("\r\n", expected), string.Join("\r\n", recorded));
            _log.Verify();
        }

        private void TestNoCallsRequired(int expected, params Mock<IDaoParameterInternal>[] mockParams)
        {
            var uut = UnderTest(mockParams);
            Assert.AreEqual(expected, uut.AccessNoCallsRequired(true));
            Assert.AreEqual(expected, uut.AccessNoCallsRequired(false));
        }

        [TestMethod]
        public void NoCallsRequired()
        {
            TestNoCallsRequired(1);
            TestNoCallsRequired(1, Input(0));
            TestNoCallsRequired(1, Input(0, true));
            TestNoCallsRequired(1, Input(1));
            TestNoCallsRequired(1, Input(1, true));
            TestNoCallsRequired(99, Input(99));
            TestNoCallsRequired(99, Input(99, true));
            TestNoCallsRequired(99, Input(99), Input(0));
            TestNoCallsRequired(99, Input(0), Input(99));
            TestNoCallsRequired(99, Input(99), Output());
            TestNoCallsRequired(99, Output(), Input(99));
            TestNoCallsRequired(108, Input(0), Input(108), Input(108), Input(108), Output(), Input(0), Input(108));
        }

        [TestMethod]
        public void NoCallsRequiredThrowExecptionMismatchLengths()
        {
            var paramList = new[] { Input(1), Input(108), Output(), Input(0) };
            int idx = 1;
            foreach (var param in paramList)
            {
                param.Setup(p => p.Name).Returns($"Param_{idx++}");
            }
            var uut = UnderTest(paramList);
            try
            {
                uut.AccessNoCallsRequired(false);
                Assert.Fail("Exception Was Expected");
            }
            catch (DaoUtilsException ex)
            {
                Assert.AreEqual(
                    "Mismatch in input array parameter lengths - Param_1: 1 values,Param_2: 108 values",
                    ex.Message);
            }
        }

        private AccessDaoCommandParameterHelper UutForExecute()
        {
            var mockParams = new[] { Input(0), Input(3), Input(3), Output(), Output() };
            var uut = UnderTest(mockParams);
            int idx = 0;
            Array.ForEach(mockParams, m =>
            {
                var param = idx++;
                m.Setup(p => p.PreCall(It.IsAny<int>()))
                  .Callback<int>(i => uut.RecordCall($"param[{param}].PreCall({i})"));
                m.Setup(p => p.PostCall()).Callback(() => uut.RecordCall($"param[{param}].PostCall()"));
                m.Setup(p => p.SetReadIndex(It.IsAny<int>()))
                  .Callback<int>(i => uut.RecordCall($"param[{param}].SetReadIndex({i})"));
                m.Setup(p => p.PreOnExecute(It.IsAny<bool>(), It.IsAny<int>()))
                  .Callback<bool, int>((b, i) => uut.RecordCall($"param[{param}].PreOnExecute({b}, {i})"));
            });
            return uut;
        }

        private void Execute(CommandExecuteMode mode, params string[] expected)
        {
            var uut = UutForExecute();
            CheckArrays.CheckEqual("Check Calls", uut.Execute(mode), expected);
        }

        [TestMethod]
        public void ExecuteNonQueryMode()
        {
            Execute(CommandExecuteMode.NonQuery,
            "param[0].PreOnExecute(False, 3)",
            "param[1].PreOnExecute(False, 3)",
            "param[2].PreOnExecute(False, 3)",
            "param[3].PreOnExecute(False, 3)",
            "param[4].PreOnExecute(False, 3)",
            "PreOnExecute(3, False)",
            "param[1].PreCall(0)",
            "param[2].PreCall(0)",
            "onExecute(0)",
            "param[3].PostCall()",
            "param[4].PostCall()",
            "param[1].PreCall(1)",
            "param[2].PreCall(1)",
            "onExecute(1)",
            "param[3].PostCall()",
            "param[4].PostCall()",
            "param[1].PreCall(2)",
            "param[2].PreCall(2)",
            "onExecute(2)",
            "param[3].PostCall()",
            "param[4].PostCall()");
        }

        [TestMethod]
        public void ExecuteQueryMode()
        {
            Execute(CommandExecuteMode.Query,
            "param[0].PreOnExecute(True, 3)",
            "param[1].PreOnExecute(True, 3)",
            "param[2].PreOnExecute(True, 3)",
            "param[3].PreOnExecute(True, 3)",
            "param[4].PreOnExecute(True, 3)",
            "PreOnExecute(3, True)",
            "param[1].PreCall(0)",
            "param[2].PreCall(0)",
            "onExecute(0)",
            "param[1].PreCall(1)",
            "param[2].PreCall(1)",
            "onExecute(1)",
            "param[1].PreCall(2)",
            "param[2].PreCall(2)",
            "onExecute(2)");
        }

        private Mock<IDaoParameterInternal> Named(string name, int inputArraySize = 0)
        {
            var param = Param();
            param.Setup(p => p.InputParamArraySize).Returns(inputArraySize);
            param.Setup(p => p.Name).Returns(name);
            return param;
        }

        private void DoValidateParameters(AccessDaoCommandParameterHelper uut, params string[] queryParams)
        {
            uut.ValidateParameters(queryParams, new string[0], false);
        }
        private void DoValidateParameters(string expectedExceptionMessage, AccessDaoCommandParameterHelper uut, params string[] queryParams)
        {
            try
            {
                DoValidateParameters(uut, queryParams);
                Assert.Fail("No Exception");
            }
            catch (DaoUtilsException e)
            {
                Assert.AreEqual(expectedExceptionMessage, e.Message);
            }
        }

        [TestMethod]
        public void ValidateParametersNoExceptionIfAllOk()
        {
            DoValidateParameters(UnderTest());
            DoValidateParameters(UnderTest(), (string[])null);

            DoValidateParameters(UnderTest(Named("First", 10)), "First");
            DoValidateParameters(UnderTest(Named("Second")), "Second");

            DoValidateParameters(UnderTest(Named("One"), Named("Two"), Named("Three")), "One", "Two", "Three");
            DoValidateParameters(UnderTest(Named("One"), Named("Two"), Named("Three")), "two", "THREE", "one");
            DoValidateParameters(UnderTest(Named("TWO"), Named("three"), Named("ONe")), "One", "Two", "Three");

            DoValidateParameters(UnderTest(Named("One", 10), Named("Two"), Named("Three", 10)), "One", "Two", "Three");
            DoValidateParameters(UnderTest(Named("One", 10), Named("Two", 10), Named("Three", 10)), "One", "Two", "Three");
        }

        [TestMethod]
        public void ValidateParametersMissingParams()
        {
            DoValidateParameters("Unknown Parameters: one", UnderTest(Named("One")));
            DoValidateParameters("Missing Parameters: one", UnderTest(), "One");
            DoValidateParameters("Unknown Parameters: one", UnderTest(Named("One"), Named("Two")), "Two");
            DoValidateParameters("Missing Parameters: one", UnderTest(Named("Two")), "One", "Two");
            DoValidateParameters("Unknown Parameters: one, three", UnderTest(Named("One"), Named("Two"), Named("Three")), "Two");
            DoValidateParameters("Missing Parameters: one, three", UnderTest(Named("Two")), "One", "Two", "Three");
        }

        [TestMethod]
        public void ValidateParametersDuplicateParams()
        {
            DoValidateParameters("Duplicated Added Parameters: one", UnderTest(Named("One"), Named("One")), "One");
            DoValidateParameters("Duplicated Query Parameters: one", UnderTest(Named("One")), "One", "One");
            DoValidateParameters("Duplicated Added Parameters: one", UnderTest(Named("One"), Named("Two"), Named("One")), "One", "Two");
            DoValidateParameters("Duplicated Query Parameters: one", UnderTest(Named("One"), Named("Two")), "One", "Two", "One");
            DoValidateParameters("Duplicated Added Parameters: one, two", UnderTest(Named("One"), Named("Two"), Named("One"), Named("Two")), "One", "Two");
            DoValidateParameters("Duplicated Query Parameters: one, two", UnderTest(Named("One"), Named("Two")), "One", "Two", "One", "Two");
        }

        [TestMethod]
        public void ValidateParametersDifferentnoParamsSetup()
        {
            DoValidateParameters("Different Number Parameter Values in Arrays: Two - 10, Three - 9", UnderTest(Named("One"), Named("Two", 10), Named("Three", 9)), "One", "Two", "Three");
            DoValidateParameters("Different Number Parameter Values in Arrays: One - 1, Two - 10, Three - 9", UnderTest(Named("One", 1), Named("Two", 10), Named("Three", 9)), "One", "Two", "Three");
        }

        [TestMethod]
        public void ValidateParametersMultipleIssues()
        {
            var expected = string.Join("\r\n", 
                "Missing Parameters: two",
                "Unknown Parameters: one",
                "Duplicated Query Parameters: two",
                "Duplicated Added Parameters: three",
                "Different Number Parameter Values in Arrays: three - 10, Three - 9");
            DoValidateParameters(expected, 
                UnderTest(Named("One"), Named("three", 10), Named("Three", 9)), "Two", "Two", "Three");
        }

        [TestMethod]
        public void ParamertersByName()
        {
            var names = new[] { "One", "Two", "Three", "Four", "Five" };
            var paramList = names.Select(p => Named(p)).ToArray();
            var uut = UnderTest(paramList);
            var byName = uut.ParamertersByName();
            CheckArrays.CheckEqual("Names", byName.Keys, names.Select(n => n));
            for (var idx = 0; idx < names.Length; idx++)
            {
                Assert.AreEqual(paramList[idx].Object, byName[names[idx]]);
            }
        }
    }
}
