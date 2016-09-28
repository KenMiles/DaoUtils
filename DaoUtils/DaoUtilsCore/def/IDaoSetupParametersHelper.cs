using System;
using System.Collections.Generic;
using System.Data;

namespace DaoUtils.Standard
{

    public interface IDaoSetupParametersHelper<out TI, out TIO, out TO> 
        where TI : IDaoParametersBuilderInput
        where TIO : IDaoParametersBuilderInputOutput
        where TO : IDaoParametersBuilderOutput
    {
        IDaoParametersBuilderDirection<TI, TIO, TO> Name(string parameterName);
        bool IgnoreQueryParamIssues { get; set; }
        void IgnoreQueryParamNames(params string[] paramNames);
    }

    public interface IDaoSetupParametersHelper<out TI>
        where TI : IDaoParametersBuilderInput
    {
        TI Name(string parameterName);
    }

}