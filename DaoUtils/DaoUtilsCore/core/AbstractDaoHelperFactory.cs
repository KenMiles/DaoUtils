using DaoUtilsCore.def;
using System;
using System.Data;
using System.Data.Common;
using Common.Logging;
using DaoUtils.code;
using DaoUtils.Standard;
using DaoUtilsCore.code;

namespace DaoUtilsCore.core
{
    public abstract class AbstractDaoHelperFactory<TH, TBldr, TCon> : IDaoHelperFactory<TH, TBldr, TCon>
        //where TH : IDaoHelper
        where TCon : IDbConnection
        where TBldr : DbConnectionStringBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AbstractDaoHelperFactory<TH, TBldr, TCon>));

        private ICoding Coding { get; }
        internal AbstractDaoHelperFactory(ICoding coding)
        {
            Coding = coding;
        }

        protected AbstractDaoHelperFactory() : this(new Coding())
        {
        }

        protected AbstractDaoHelperFactory(string applicationEncryptionKey) : this(new Coding(applicationEncryptionKey))
        {
        }


        public static string GenerateApplicationEncryptionKey()
        {
            return new Coding().GenerateCodeKeyString();
        }

        public string EncryptPassword(string password)
        {
            return Coding.EncryptStr(password);
        }

        abstract protected TH CreateHelper(TCon connection, OpenConnection openConnection = OpenConnection.Background);

        abstract protected TCon CreateConnection(string connectionString);

        protected virtual void SetPassword(TBldr connectionStringBuilder, string password)
        {
            connectionStringBuilder["Password"] = password;
        }

        public TH Helper(TCon connection, OpenConnection openConnection = OpenConnection.Background)
        {
            try
            {
                return CreateHelper(connection, openConnection);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public TH Helper(TBldr connectionStringBuilder, OpenConnection openConnection = OpenConnection.Background)
        {
            try
            {
                return Helper(CreateConnection(connectionStringBuilder.ConnectionString), openConnection);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public TH Helper(TBldr connectionStringBuilder, string encryptedPassword, OpenConnection openConnection = OpenConnection.Background)
        {
            try
            {
                SetPassword(connectionStringBuilder, Coding.DecryptString(encryptedPassword));
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            return Helper(connectionStringBuilder, openConnection);
        }

    }
}
