using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using DaoUtils.def;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using DaoUtilsCore.def;
using DaoUtilsCore.def.ms;

namespace DaoUtilsCore.ms
{
    public interface IMsDaoHelperFactory : IDaoHelperFactory<IMsDaoHelper, SqlConnectionStringBuilder, SqlConnection>
    {
    }

    class MsDaoHelperFactory : AbstractDaoHelperFactory<IMsDaoHelper, SqlConnectionStringBuilder, SqlConnection>, IMsDaoHelperFactory
    {
        protected override SqlConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override IMsDaoHelper CreateHelper(SqlConnection connection, OpenConnection openConnection = OpenConnection.Background)
        {
            return new MsDaoHelper(connection, openConnection);
        }
    }
}
