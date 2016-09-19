namespace DaoUtilsCore.log.console
{
    internal interface IConsolWrapper
    {
        void WriteLn( string message, bool useErrorStream);
        void WriteLnToDebug(string message);
    }
}