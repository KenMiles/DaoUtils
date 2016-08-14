using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DaoUtilsCore.def
{
    internal interface IColumnHolder
    {
        string ColumnName { get; }
        int NoValues { get; }
        object Value(int index);
        DataColumn CreateColumn();
    }
}
