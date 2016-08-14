using DaoUtils.Standard;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DaoUtilsCore.def
{
    public interface IDaoHelperFactory<out TH, in TBldr, in TCon>
        //where TH : IDaoHelper
        where TCon : IDbConnection
        where TBldr : DbConnectionStringBuilder
    {
        TH Helper(TCon connection, OpenConnection openConnection = OpenConnection.Background);
        TH Helper(TBldr connectionStringBuilder, OpenConnection openConnection = OpenConnection.Background);
        TH Helper(TBldr connectionStringBuilder, string encryptedPassword, OpenConnection openConnection = OpenConnection.Background);
    }
}
