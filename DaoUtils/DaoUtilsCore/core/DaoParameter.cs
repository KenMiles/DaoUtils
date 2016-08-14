using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DaoUtils.Standard;

namespace DaoUtils.core
{

    class DaoParameter<T> : IDaoParameter, IDaoParameterInternal, IDaoInputParameter<T>, IDaoInputOutputParameter<T>, IDaoOutputParameter<T>
    {

        private List<T> _inputValues;
        private List<T> _outputValues;
        protected List<T> InputValues => _inputValues;
        protected int Size { get; private set; }
        private readonly ParameterDirection _direction;
        private int _readIndex;
        public string Name { get; }
        public DaoParameter(IDbDataParameter dbDataParameter, string name, ParameterDirection direction, int size)
        {
            Parameter = dbDataParameter;
            Name = name;
            _direction = direction;
            Size = size;
        }

        public IDbDataParameter Parameter { get ; }

        public static bool IsNull(T value)
        {
            return ReferenceEquals(value, null);
        }

        protected void SetParameterValue(T value)
        {
            Parameter.Value = IsNull(value) ? (object)DBNull.Value : (object)value;
        }

        public DaoParameter<T> SetValue(T value)
        {
            SetParameterValue(value);
            _inputValues = null;
            return this;
        }

        IDaoInputParameter<T> IDaoInputParameter<T>.SetValue(T value)
        {
            return SetValue(value);
        }

        virtual protected T GetParameterValue()
        {
            var val = Parameter.Value;
            return (val == null || val == DBNull.Value) ? default(T) : (T)val;
        }

        public void SetReadIndex(int index)
        {
            _readIndex = index;
        }

        public T GetValue()
        {
            if (_outputValues != null && _outputValues.Count != 0) return _outputValues[_readIndex];
            return GetParameterValue();
        }

        public object GetValueAsObject()
        {
            if (_outputValues == null || _outputValues.Count == 0)
            {
                return GetParameterValue();
            }
            return _outputValues[_readIndex];
        }

        public DaoParameter<T> SetValues(IEnumerable<T> values)
        {
            _inputValues = values?.ToList() ?? new List<T>();
            return this;
        }

        IDaoInputParameter<T> IDaoInputParameter<T>.SetValues(IEnumerable<T> values)
        {
            return SetValues(values);
        }


        public string ForLog(bool readReturnValuesStage)
        {
            //TODO get param values?
            return $"{Name} {_direction}: TBI";
        }


        T IDaoInputOutputParameter<T>.Value
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        IEnumerable<T> IDaoOutputParameter<T>.Values => Values();

        T IDaoOutputParameter<T>.Value => GetValue();

        protected List<T> Values()
        {
            return _outputValues = _outputValues ?? new List<T>();
        }

        IEnumerable<T> IDaoInputOutputParameter<T>.Values
        {
            get { return Values(); }
            set { SetValues(value); }
        }

        T IDaoInputParameter<T>.Value
        {
            set { SetValue(value); }
        }

        IEnumerable<T> IDaoInputParameter<T>.Values
        {
            set { SetValues(value); }
        }

        public int InputParamArraySize => _inputValues?.Count ?? 0;

        public int OutputParamArraySize => _outputValues?.Count ?? 0;

        public bool IsInput => _direction == ParameterDirection.Input || _direction == ParameterDirection.InputOutput;

        public bool IsOutput => 
            _direction == ParameterDirection.Output 
                || _direction == ParameterDirection.ReturnValue 
                || _direction == ParameterDirection.InputOutput;

        virtual public void PreCall(int callIndex)
        {
            if (!IsInput || InputParamArraySize == 0) return;
            SetParameterValue(_inputValues[callIndex]);
        }

        virtual public void PostCall()
        {
            if (!IsOutput ) return;
            Values().Add(GetParameterValue());
        }

        virtual public void PreOnExecute(bool isQuery, int numberOfCalls)
        {
            _outputValues = null;
        }
    }
}
