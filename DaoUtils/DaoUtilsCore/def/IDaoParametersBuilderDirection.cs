using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaoUtils.Standard
{
    public interface IDaoParametersBuilderDirection<out TI, out TIO, out TO>
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
    {
        IDaoParametersBuilderDirection<TI, TIO, TO> Size(int size);
        TI Input { get; }
        TIO InputOutput { get; }
        TO Output { get; }
        TO ReturnValue { get; }
    }

}
