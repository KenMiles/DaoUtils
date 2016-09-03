using System;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using DaoUtilsCore.ms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.ms
{
    class MsDaoHelperFactoryAccess: MsDaoHelperFactory
    {
        public SqlConnection AccessCreateConnection(string connectionString)
        {
            return CreateConnection(connectionString);
        }

        public SqlConnectionStringBuilder AccessCreateConnectionStringBuilder(string connectionString)
        {
            return CreateConnectionStringBuilder(connectionString);
        }

        public void AccessSetPassword(SqlConnectionStringBuilder builder, string password)
        {
            SetPassword(builder, password);
        }
    }

    [TestClass]
    public class MsDaoHelperFactoryTests
    {

        private readonly MsDaoHelperFactoryAccess _uut = new MsDaoHelperFactoryAccess();

        [TestMethod]
        public void CreateConnection()
        {
            var connectionString = "Data Source=LOCAL\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";
            var connection = _uut.AccessCreateConnection(connectionString);
            Assert.IsNotNull(connection);
            Assert.AreEqual(connectionString, connection.ConnectionString);
        }

        [TestMethod]
        public void CreateConnectionStringBuilder()
        {
            var connectionString = "Data Source=LOCAL\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True";
            var builder = _uut.AccessCreateConnectionStringBuilder(connectionString);
            Assert.IsNotNull(builder);
            Assert.AreEqual(connectionString, builder.ConnectionString);
        }

        [TestMethod]
        public void SetPassword()
        {
            var connectionString = "Data Source=LOCAL\\SQLEXPRESS;Initial Catalog=Test;";
            var builder = new SqlConnectionStringBuilder(connectionString);
            _uut.AccessSetPassword(builder, "A Password");
            Assert.AreEqual("A Password", builder.Password);
            Assert.AreEqual($"{connectionString}Password=\"A Password\"", builder.ConnectionString);
        }
    }
}
