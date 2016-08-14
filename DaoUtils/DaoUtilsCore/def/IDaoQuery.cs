using System;
using System.Collections.Generic;
using System.Data;

namespace DaoUtils.Standard
{
    public interface IDaoQuery<out TR, out TCmd> : IDisposable
        where TR : IReadValue
        where TCmd : IDbCommand
    {
        TCmd Command { get; }

        List<T> ReadQuery<T>(DaoReadRow<T, TR> readRow);
        List<T> ReadQuery<T>(DaoReadRowAndParams<T, TR> readRow);
        Dictionary<TK, TD> ReadQuery<TK, TD>(DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow);
        Dictionary<TK, TD> ReadQuery<TK, TD>(DaoReadRowAndParams<TK, TR> rowKey, DaoReadRowAndParams<TD, TR> readRow);
        T ReadSingleRow<T>(DaoReadRow<T, TR> readRow, T defaultValue = default(T));
        T ReadSingleRow<T>(DaoReadRowAndParams<T, TR> readRow, T defaultValue = default(T));
        object ExecuteScalar();
        T ExecuteScalar<T>(T defaultValue = default(T)) where T : IConvertible;
    }
}