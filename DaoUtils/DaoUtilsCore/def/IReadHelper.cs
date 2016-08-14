using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{
    public interface IReadHelper<out T> where T : IReadValue
    {
        T Named(string name);
        T this[string name] { get; }
        T this[int columIndex] { get; }
    }

}
