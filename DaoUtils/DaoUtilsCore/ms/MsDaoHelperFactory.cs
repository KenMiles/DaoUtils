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

    public class MsDaoHelperFactory : AbstractDaoHelperFactory<IMsDaoHelper, SqlConnectionStringBuilder, SqlConnection>, IMsDaoHelperFactory
    {
        public MsDaoHelperFactory()
        {
        }

        public MsDaoHelperFactory(string applicationEncryptionKey) : base(applicationEncryptionKey)
        {
        }

        protected override SqlConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override SqlConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            return new SqlConnectionStringBuilder(connectionString);
        }

        protected override void SetPassword(SqlConnectionStringBuilder connectionStringBuilder, string password)
        {
            connectionStringBuilder.Password = password;
        }

        protected override IMsDaoHelper CreateHelper(SqlConnection connection, OpenConnection openConnection = OpenConnection.Background)
        {
            return new MsDaoHelper(connection, openConnection);
        }
    }
}
