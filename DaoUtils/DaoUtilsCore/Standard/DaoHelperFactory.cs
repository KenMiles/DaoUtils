using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaoUtilsCore.log;
using DaoUtils.code;
using DaoUtils.def;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using DaoUtilsCore.def;

namespace DaoUtils.Standard
{
    public interface IStandardDaoHelperFactory : IDaoHelperFactory<IStandardDaoHelper, DbConnectionStringBuilder, IDbConnection>
    {
    }

    public class DaoHelperFactory : AbstractDaoHelperFactory<IStandardDaoHelper, DbConnectionStringBuilder, IDbConnection>, IStandardDaoHelperFactory
    {
        public DaoHelperFactory() 
        {
        }

        public DaoHelperFactory(string applicationEncryptionKey) : base(applicationEncryptionKey)
        {
        }

        protected override IStandardDaoHelper CreateHelper(IDbConnection connection, OpenConnection openConnection = OpenConnection.Background)
        {
            return new DaoHelper(connection, openConnection);
        }

        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override DbConnectionStringBuilder CreateConnectionStringBuilder(string connectionString)
        {
            return new DbConnectionStringBuilder() {ConnectionString = connectionString};
        }
    }
}
