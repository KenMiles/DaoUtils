using System;
using System.Data;
using System.Data.Common;
using DaoUtils.Standard;
using DaoUtilsCore.code;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    /* Dummy Helper type class check parameters passed 
     - note no restrictions on generics for Helper returned as getting too complex with layers of generics */

    internal class MockHelper
    {
        public TestConnection Connection { get; set; }
        public OpenConnection OpenConnection { get; set; }
    }

    //Dummy connection class check parameters passed
    internal class TestConnection : IDbConnection
    {
        public string Password { get; set; }
        public string PassConnectionStr { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public string ConnectionString
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int ConnectionTimeout
        {
            get { throw new NotImplementedException(); }
        }

        public string Database
        {
            get { throw new NotImplementedException(); }
        }

        public ConnectionState State
        {
            get { throw new NotImplementedException(); }
        }
    }


    internal class TestableFactory : AbstractDaoHelperFactory<MockHelper, DbConnectionStringBuilder, TestConnection>
    {
        public TestableFactory(ICoding coding) : base(coding)
        {
        }

        protected override TestConnection CreateConnection(string connectionString)
        {
            return new TestConnection {PassConnectionStr = connectionString};
        }

        protected override MockHelper CreateHelper(TestConnection connection,
            OpenConnection openConnection = OpenConnection.Background)
        {
            return new MockHelper {Connection = connection, OpenConnection = openConnection};
        }

        /*protected override TestConnection CreateConnection(DbConnectionStringBuilder connectionStringBuilder, string password)
        {
            return new TestConnection {ConnectionStringBuilder = connectionStringBuilder, Password = password};
        }*/
    }


    [TestClass]
    public class AbstractDaoHelperFactoryTests
    {
        private readonly Mock<ICoding> _coding = new Mock<ICoding>(MockBehavior.Strict);
        private TestableFactory _factory;

        [TestInitialize]
        public void Setup()
        {
            _factory = new TestableFactory(_coding.Object);
        }

        [TestMethod]
        public void EncryptPassword()
        {
            const string password = "A Password to encrypt";
            const string encryptedPassword = "An Encrypted Password";
            _coding.Setup(c => c.EncryptStr(It.IsAny<string>())).Returns(encryptedPassword)
                .Callback<string>(p => { Assert.AreEqual(password, p); }).Verifiable();

            Assert.AreEqual(encryptedPassword, _factory.EncryptPassword(password));
            _coding.Verify();
        }

        private void CheckHelperFromConnection(OpenConnection openConnectionWhen)
        {
            var connection = new TestConnection();
            var helper = _factory.Helper(connection, openConnectionWhen);
            Assert.IsNotNull(helper);
            Assert.AreEqual(connection, helper.Connection);
            Assert.AreEqual(openConnectionWhen, helper.OpenConnection);
            Assert.IsNull(helper.Connection.Password);
        }

        [TestMethod]
        public void HelperFromConnection()
        {
            CheckHelperFromConnection(OpenConnection.Background);
            CheckHelperFromConnection(OpenConnection.Immediate);
            CheckHelperFromConnection(OpenConnection.FirstAccess);
        }

        private void CheckHelperFromBuilder(OpenConnection openConnectionWhen, string password, string encryptedPassword)
        {
            _coding.Setup(c => c.DecryptString(It.IsAny<string>())).Returns(password)
                .Callback<string>(p => { Assert.AreEqual(encryptedPassword, p); }).Verifiable();
            var builder = new DbConnectionStringBuilder
            {
                ["Hello"] = "Hi There",
                ["Goodbye"] = "See You"
            };
            var helper = _factory.Helper(builder, encryptedPassword, openConnectionWhen);
            Assert.IsNotNull(helper);
            Assert.AreEqual(openConnectionWhen, helper.OpenConnection);
            Assert.AreEqual(password, builder["Password"]);
            Assert.IsFalse(string.IsNullOrWhiteSpace(builder.ConnectionString));
            Assert.AreEqual(builder.ConnectionString, helper.Connection.PassConnectionStr);
            _coding.Verify();
        }

        [TestMethod]
        public void HelperFromBuilderEnryptedPassword()
        {
            CheckHelperFromBuilder(OpenConnection.Background, "A Password", "And then Encrypted");
            CheckHelperFromBuilder(OpenConnection.Immediate, "A Different Password", "And then Encrypted again");
            CheckHelperFromBuilder(OpenConnection.FirstAccess, "Yet another Password", "And Encrypted yet again");
        }

        private void CheckHelperFromBuilder(OpenConnection openConnectionWhen)
        {
            var builder = new DbConnectionStringBuilder
            {
                ["How"] = "Are You",
                ["Good"] = "Today"
            };
            var helper = _factory.Helper(builder, openConnectionWhen);
            Assert.IsNotNull(helper);
            Assert.AreEqual(openConnectionWhen, helper.OpenConnection);
            Assert.IsFalse(builder.ContainsKey("Password"));
            Assert.IsFalse(string.IsNullOrWhiteSpace(builder.ConnectionString));
            Assert.AreEqual(builder.ConnectionString, helper.Connection.PassConnectionStr);
            _coding.Verify();
        }

        [TestMethod]
        public void HelperFromBuilder()
        {
            CheckHelperFromBuilder(OpenConnection.Background);
            CheckHelperFromBuilder(OpenConnection.Immediate);
            CheckHelperFromBuilder(OpenConnection.FirstAccess);
        }
    }
}