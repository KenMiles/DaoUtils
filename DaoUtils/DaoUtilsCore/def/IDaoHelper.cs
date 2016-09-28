using System;
using System.Collections.Generic;
using System.Data;

namespace DaoUtils.Standard
{
    public enum OpenConnection { Background, Immediate, FirstAccess }
    public delegate T DaoReadRow<out T, in TR>(IReadHelper<TR> drHelper) where TR: IReadValue;
    public delegate T DaoReadRowAndParams<out T, in TR>(IReadHelper<TR> drHelper, IReadHelper<TR> parameterHelper) where TR : IReadValue;
    public delegate void DaoSetupParameters<in TI, in TIO, in TO>(IDaoSetupParametersHelper<TI, TIO, TO> parameterHelper)
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
    ;
    public delegate void DaoSetupParameters<in TI>(IDaoSetupParametersHelper<TI> parameterHelper)
        where TI : IDaoParametersBuilderInput
    ;
    public delegate void DaoOnExecute<in TR>(IReadHelper<TR> paraReadHelper) where TR : IReadValue;
    public delegate T DaoOnExecute<out T, in TR>(IReadHelper<TR> paraReadHelper, int numberRowsAffected) where TR : IReadValue;

    public interface IDaoConnectionInfo
    {
        IDbTransaction ActiveTransaction { get; }
        void WaitOpen();
        string ParamPrefix { get; }
    }

    public interface IDaoHelper<out TI, out TIO, out TO, TR, TCmd> : IDisposable
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
        where TR : IReadValue
        where TCmd : IDbCommand
    {
        IDaoCommand<TR, TCmd> NewCommand(string commandText, DaoSetupParameters<TI, TIO, TO> setupParameters);
        IDaoCommand<TR, TCmd> NewCommand(string commandText);

        IDaoCommand<TR, TCmd> NewStoredProc(string procedureName, DaoSetupParameters<TI, TIO, TO> setupParameters);
        IDaoCommand<TR, TCmd> NewStoredProc(string procedureName);

        IDaoQuery<TR, TCmd> NewQuery(string querySql, DaoSetupParameters<TI> setupParameters);
        IDaoQuery<TR, TCmd> NewQuery(string querySql);

        void DoInTransaction(string description, Action daoInTransaction);

        List<T> ReadQuery<T>(IDaoQuery<TR, TCmd> command, DaoReadRow<T, TR> readRow);
        List<T> ReadQuery<T>(string sql, DaoReadRow<T, TR> readRow);
        List<T> ReadQuery<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<T, TR> readRow);
        List<T> ReadQuery<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<T, TR> readRow);

        T ReadSingleRow<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<T, TR> readRow, T defaultValue = default(T));
        T ReadSingleRow<T>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<T, TR> readRow, T defaultValue = default(T));
        T ReadSingleRow<T>(string sql, DaoReadRow<T, TR> readRow, T defaultValue = default(T));

        int? ReadSingleIntNullable(string sql);
        DateTime? ReadSingleDateNullable(string sql);
        int ReadSingleInt(string sql, int defaultValue = 0);
        DateTime ReadSingleDate(string sql, DateTime defaultValue = default(DateTime));
        string ReadSingleString(string sql, string defaultValue = null);

        int? ReadSingleIntNullable(string sql, DaoSetupParameters<TI> setupParameters);
        DateTime? ReadSingleDateNullable(string sql, DaoSetupParameters<TI> setupParameters);
        int ReadSingleInt(string sql, DaoSetupParameters<TI> setupParameters, int defaultValue = 0);
        DateTime ReadSingleDate(string sql, DaoSetupParameters<TI> setupParameters, DateTime defaultValue = default(DateTime));
        string ReadSingleString(string sql, DaoSetupParameters<TI> setupParameters, string defaultValue = null);

        Dictionary<TK, TD> ReadQuery<TK, TD>(IDaoQuery<TR, TCmd> command, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow);
        Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow);
        Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRow<TK, TR> rowKey, DaoReadRow<TD, TR> readRow);
        Dictionary<TK, TD> ReadQuery<TK, TD>(string sql, DaoSetupParameters<TI> setupParameters, DaoReadRowAndParams<TK, TR> rowKey, DaoReadRowAndParams<TD, TR> readRow);

        int[] ExecuteNonQuery(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters);
        int[] ExecuteNonQuery(string sql);
        int[] ExecuteNonQuery(IDaoCommand<TR, TCmd> command);
        //int[] ExecuteNonQuery(TCmd command);

        List<T> ExecuteNonQuery<T>(string sql, DaoSetupParameters<TI, TIO, TO> setupParameters, DaoOnExecute<T,TR> onExecute);
        List<T> ExecuteNonQuery<T>(string sql, DaoOnExecute<T, TR> onExecute);
        List<T> ExecuteNonQuery<T>(IDaoCommand<TR, TCmd> command, DaoOnExecute<T, TR> onExecute);

        int[] ExecuteStoredProc(string storedProcName, DaoSetupParameters<TI, TIO, TO> setupParameters);
        int[] ExecuteStoredProc(string storedProcName);

        List<T> ExecuteStoredProc<T>(string storedProcName, DaoSetupParameters<TI, TIO, TO> setupParameters, DaoOnExecute<T, TR> onExecute);
        List<T> ExecuteStoredProc<T>(string storedProcName, DaoOnExecute<T, TR> onExecute);
    }



}