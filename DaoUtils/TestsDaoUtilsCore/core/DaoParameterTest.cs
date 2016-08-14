using System;
using System.Collections.Generic;
using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    internal class AccessableDaoParameter<T> : DaoParameter<T>
    {
        public AccessableDaoParameter(IDbDataParameter dbDataParameter, string name, ParameterDirection direction,
            int size) : base(dbDataParameter, name, direction, size)
        {
        }

        public List<T> InputListValues => InputValues;

        public T AccessGetParameterValue()
        {
            return GetParameterValue();
        }

        public List<T> OutputListValues => Values();
    }

    [TestClass]
    public class DaoParameterTest
    {
        private readonly Mock<IDbDataParameter> _mockDbParam = new Mock<IDbDataParameter>(MockBehavior.Strict);
        private const string ParamName = "A Parameter Name";

        private AccessableDaoParameter<T> Uut<T>(ParameterDirection direction = ParameterDirection.Input, int size = 0)
        {
            return new AccessableDaoParameter<T>(_mockDbParam.Object, ParamName, direction, size);
        }

        private AccessableDaoParameter<string> Uut(ParameterDirection direction = ParameterDirection.Input, int size = 0)
        {
            return Uut<string>(direction, size);
        }

        [TestMethod]
        public void IsNull()
        {
            Assert.IsTrue(DaoParameter<string>.IsNull(null));
            Assert.IsFalse(DaoParameter<string>.IsNull(""));
            Assert.IsFalse(DaoParameter<string>.IsNull(" "));
            Assert.IsFalse(DaoParameter<string>.IsNull("Some Value"));
            Assert.IsTrue(DaoParameter<int?>.IsNull(null));
            Assert.IsFalse(DaoParameter<int?>.IsNull(101));
            Assert.IsFalse(DaoParameter<int?>.IsNull(0));
            Assert.IsFalse(DaoParameter<int>.IsNull(0));
        }

        [TestMethod]
        public void SetParameterValue()
        {
            var list = new List<object>();
            _mockDbParam.SetupSet(p => p.Value = It.IsAny<object>()).Callback<object>(o => list.Add(o));
            var uut = Uut();
            uut.SetValues(_values);
            Assert.IsNotNull(uut.InputListValues);

            Assert.AreEqual(uut, uut.SetValue(""));
            Assert.AreEqual(uut, uut.SetValue("One Value"));
            Assert.AreEqual(uut, uut.SetValue(null));
            //Set value should have zapped array InputValues
            Assert.IsNull(uut.InputListValues);

            var uutInt = Uut<int?>();
            Assert.AreEqual(uutInt, uutInt.SetValue(0));
            Assert.AreEqual(uutInt, uutInt.SetValue(null));
            Assert.AreEqual(uutInt, uutInt.SetValue(999));

            CollectionAssert.AreEqual(new object[] {"", "One Value", DBNull.Value, 0, DBNull.Value, 999}, list);
        }

        [TestMethod]
        public void SetParameterValueDaoInputParameter()
        {
            var list = new List<object>();
            _mockDbParam.SetupSet(p => p.Value = It.IsAny<object>()).Callback<object>(o => list.Add(o));
            IDaoInputParameter<string> uut = Uut();
            Assert.AreEqual(uut, uut.SetValue(null));
            Assert.AreEqual(uut, uut.SetValue(""));
            Assert.AreEqual(uut, uut.SetValue("Different Value"));

            CollectionAssert.AreEqual(new object[] {DBNull.Value, "", "Different Value"}, list);
        }

        [TestMethod]
        public void GetParameterValue()
        {
            var uut = Uut();
            _mockDbParam.Setup(p => p.Value).Returns("A Value");
            Assert.AreEqual("A Value", uut.AccessGetParameterValue());
            _mockDbParam.Setup(p => p.Value).Returns("");
            Assert.AreEqual("", uut.AccessGetParameterValue());
            _mockDbParam.Setup(p => p.Value).Returns(null);
            Assert.IsNull(uut.AccessGetParameterValue());
            _mockDbParam.Setup(p => p.Value).Returns(DBNull.Value);
            Assert.IsNull(uut.AccessGetParameterValue());

            var uutInt = Uut<int>();
            _mockDbParam.Setup(p => p.Value).Returns(null);
            Assert.AreEqual(0, uutInt.AccessGetParameterValue());
            _mockDbParam.Setup(p => p.Value).Returns(DBNull.Value);
            Assert.AreEqual(0, uutInt.AccessGetParameterValue());
            _mockDbParam.Setup(p => p.Value).Returns(2);
            Assert.AreEqual(2, uutInt.AccessGetParameterValue());

        }

        private readonly string[] _values = new[] {"One", "Two", "Three", "Four"};

        [TestMethod]
        public void GetValue()
        {
            var uut = Uut();
            _mockDbParam.Setup(p => p.Value).Returns("A Value");

            Assert.AreEqual("A Value", uut.GetValue());
            uut.SetReadIndex(990);
            Assert.AreEqual("A Value", uut.GetValue());
            uut.OutputListValues.AddRange(_values);
            uut.SetReadIndex(0);
            Assert.AreEqual("One", uut.GetValue());
            uut.SetReadIndex(1);
            Assert.AreEqual("Two", uut.GetValue());
            uut.SetReadIndex(2);
            Assert.AreEqual("Three", uut.GetValue());
            uut.SetReadIndex(3);
            Assert.AreEqual("Four", uut.GetValue());

            uut.OutputListValues.Clear();
            Assert.AreEqual("A Value", uut.GetValue());
        }

        [TestMethod]
        public void GetValueAsObject()
        {
            var uut = Uut();
            _mockDbParam.Setup(p => p.Value).Returns("A Value");

            Assert.AreEqual("A Value", uut.GetValueAsObject());
            uut.SetReadIndex(990);
            Assert.AreEqual("A Value", uut.GetValueAsObject());
            uut.OutputListValues.AddRange(_values);
            uut.SetReadIndex(0);
            Assert.AreEqual("One", uut.GetValueAsObject());
            uut.SetReadIndex(1);
            Assert.AreEqual("Two", uut.GetValueAsObject());
            uut.SetReadIndex(2);
            Assert.AreEqual("Three", uut.GetValueAsObject());
            uut.SetReadIndex(3);
            Assert.AreEqual("Four", uut.GetValueAsObject());

            uut.OutputListValues.Clear();
            Assert.AreEqual("A Value", uut.GetValueAsObject());
        }

        [TestMethod]
        public void SetValues()
        {
            var uut = Uut();
            uut.SetValues(null);
            Assert.AreEqual(0, uut.InputListValues?.Count);
            Assert.AreEqual(uut, uut.SetValues(_values));
            CollectionAssert.AreEqual(_values, uut.InputListValues);
        }

        [TestMethod]
        public void SetValuesIDaoInputParameter()
        {
            var uutRaw = Uut();
            IDaoInputParameter<string> uut = uutRaw;
            uut.SetValues(null);
            Assert.AreEqual(0, uutRaw.InputListValues?.Count);
            Assert.AreEqual(uut, uut.SetValues(_values));
            CollectionAssert.AreEqual(_values, uutRaw.InputListValues);
        }

        [TestMethod]
        public void ForLog()
        {
            Assert.AreEqual($"{ParamName} {ParameterDirection.Input}: TBI", Uut(ParameterDirection.Input).ForLog(true));
            Assert.AreEqual($"{ParamName} {ParameterDirection.Input}: TBI", Uut(ParameterDirection.Input).ForLog(false));
            Assert.AreEqual($"{ParamName} {ParameterDirection.InputOutput}: TBI",
                Uut(ParameterDirection.InputOutput).ForLog(false));
        }

        [TestMethod]
        public void IdaoInputOutputParameterValue()
        {
            _mockDbParam.SetupSet(p => p.Value = "An Input Value").Verifiable();
            _mockDbParam.SetupGet(p => p.Value).Returns("An Output Value").Verifiable();
            var rawUut = Uut();
            rawUut.SetValues(_values);
            IDaoInputOutputParameter<string> uut = rawUut;
            Assert.AreEqual("An Output Value", uut.Value);
            uut.Value = "An Input Value";
            _mockDbParam.Verify();
            Assert.IsNull(rawUut.InputListValues);

            rawUut.OutputListValues.AddRange(_values);
            rawUut.SetReadIndex(2);
            Assert.AreEqual(_values[2], uut.Value);
            rawUut.SetReadIndex(1);
            Assert.AreEqual(_values[1], uut.Value);

        }

        [TestMethod]
        public void IdaoInputParameterValue()
        {
            _mockDbParam.SetupSet(p => p.Value = "An Input Value").Verifiable();
            var rawUut = Uut();
            rawUut.SetValues(_values);
            Assert.IsNotNull(rawUut.InputListValues);
            IDaoInputParameter<string> uut = rawUut;
            uut.Value = "An Input Value";
            _mockDbParam.Verify();
            Assert.IsNull(rawUut.InputListValues);
        }

        [TestMethod]
        public void IdaoInputParameterValues()
        {
            var rawUut = Uut();
            Assert.IsNull(rawUut.InputListValues);
            IDaoInputParameter<string> uut = rawUut;
            uut.Values = _values;
            CollectionAssert.AreEqual(_values, rawUut.InputListValues);
        }

        [TestMethod]
        public void InputParamArraySize()
        {
            var uut = Uut();
            Assert.AreEqual(0, uut.InputParamArraySize);
            uut.SetValues(_values);
            Assert.AreEqual(_values.Length, uut.InputParamArraySize);
        }

        [TestMethod]
        public void OutputParamArraySize()
        {
            var uut = Uut();
            Assert.AreEqual(0, uut.OutputParamArraySize);
            uut.OutputListValues.AddRange(_values);
            Assert.AreEqual(_values.Length, uut.OutputParamArraySize);
        }

        private void CheckIsInputAndIsOutput(ParameterDirection direction, bool isInput, bool isOutput)
        {
            var uut = Uut(direction);
            Assert.AreEqual(isInput, uut.IsInput);
            Assert.AreEqual(isOutput, uut.IsOutput);
        }

        [TestMethod]
        public void IsInputAndIsOutput()
        {
            CheckIsInputAndIsOutput(ParameterDirection.Input, true, false);
            CheckIsInputAndIsOutput(ParameterDirection.InputOutput, true, true);
            CheckIsInputAndIsOutput(ParameterDirection.Output, false, true);
            CheckIsInputAndIsOutput(ParameterDirection.ReturnValue, false, true);
        }

        [TestMethod]
        public void PreCallInputAndArrayValues()
        {
            _mockDbParam.SetupSet(p => p.Value = _values[3]).Verifiable();
            var uut = Uut();
            uut.SetValues(_values);
            uut.PreCall(3);
            _mockDbParam.Verify();
        }

        [TestMethod]
        public void PreCallInputOutputAndArrayValues()
        {
            _mockDbParam.SetupSet(p => p.Value = _values[3]).Verifiable();
            var uut = Uut(ParameterDirection.InputOutput);
            uut.SetValues(_values);
            uut.PreCall(3);
            _mockDbParam.Verify();
        }

        [TestMethod]
        public void PreCallInputSingleValue()
        {
            var uut = Uut();
            uut.PreCall(3);
        }

        [TestMethod]
        public void PreCallOutputAndArrayValues()
        {
            var uut = Uut(ParameterDirection.Output);
            uut.SetValues(_values);
            uut.PreCall(3);
        }

        private void CheckPostCall(ParameterDirection direction, params string[] expected)
        {
            var uut = Uut(direction);
            uut.PostCall();
            CollectionAssert.AreEqual(expected, uut.OutputListValues);
        }

        [TestMethod]
        public void PostCall()
        {
            _mockDbParam.SetupGet(p => p.Value).Returns("A Value");
            CheckPostCall(ParameterDirection.Input);
            CheckPostCall(ParameterDirection.InputOutput, "A Value");
            CheckPostCall(ParameterDirection.Output, "A Value");
            CheckPostCall(ParameterDirection.ReturnValue, "A Value");
        }

        [TestMethod]
        public void PreOnExecute()
        {
            var uut = Uut();
            uut.OutputListValues.AddRange(_values);
            uut.PreOnExecute(true, 1);
            Assert.AreEqual(0, uut.OutputListValues.Count);

        }
    }
}