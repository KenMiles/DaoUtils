using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{
    public interface IDaoCommand<out TR, out TCmd> : IDisposable
        where TR : IReadValue
        where TCmd: IDbCommand
    {
        TCmd Command { get; }

        int[] ExecuteNonQuery();
        List<T> ExecuteNonQuery<T>(DaoOnExecute<T, TR> onExecute);
    }
}
