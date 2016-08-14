using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//[assembly: CLSCompliant(true)]

namespace DaoUtils.Standard
{
    //[CLSCompliant(true)]
    public interface IDaoParameter
    {
        string Name { get; }
        IDbDataParameter Parameter { get; }

    }

    internal interface IDaoParameterInternal : IDaoParameter
    {
        int InputParamArraySize { get; }
        int OutputParamArraySize { get; }
        bool IsOutput { get; }
        bool IsInput { get; }
        void PreCall(int callIndex);
        void PreOnExecute(bool isQuery, int numberOfCalls);
        void PostCall();
        void SetReadIndex(int index);
        object GetValueAsObject();
        string ForLog(bool readReturnValuesStage);
    }

    public interface IDaoInputParameter<in T> : IDaoParameter
    {
        IDaoInputParameter<T> SetValue(T value);
        IDaoInputParameter<T> SetValues(IEnumerable<T> value);
        T Value { set; }
        IEnumerable<T> Values { set; }
    }

    public interface IDaoOutputParameter<out T> : IDaoParameter
    {
        T Value { get; }
        IEnumerable<T> Values { get; }
    }

    public interface IDaoInputOutputParameter<T> : IDaoInputParameter<T>
    {
        new T Value { get; set; }
        new IEnumerable<T> Values { get; set; }
    }


}
