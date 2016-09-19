using System;

namespace DaoUtilsCore.log.console
{
    internal class ConsolWrapper: IConsolWrapper
    {
        public void WriteLn(string message, bool useErrorStream)
        {
            if (useErrorStream)
            {
                Console.Error.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public void WriteLnToDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}