using System;
using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.standard
{
    internal class AccessDataReaderHelper : DataReaderHelper
    {
        public AccessDataReaderHelper(IDataReader reader) : base(reader)
        {
        }

        public IReadValue AccessCreateColumnReader(IDataReader reader, int columnIndex)
        {
            return CreateColumnReader(reader, columnIndex);
        }
    }

    [TestClass]
    public class DataReaderHelperTests
    {
        private readonly Mock<IDataReader> _readerForConstructor = new Mock<IDataReader>(MockBehavior.Strict);
        private readonly Mock<IDataReader> _readerForMethod = new Mock<IDataReader>(MockBehavior.Strict);
        private AccessDataReaderHelper _uut;

        [TestInitialize]
        public void SetUp()
        {
            _uut = new AccessDataReaderHelper(_readerForConstructor.Object);
        }


        [TestMethod]
        public void CreateColumnReader()
        {
            var reader = _uut.AccessCreateColumnReader(_readerForMethod.Object, 998);
            Assert.IsNotNull(reader as ColumnReadValue);
            _readerForMethod.Setup(r => r.IsDBNull(998)).Returns(true).Verifiable();
            Assert.IsTrue(reader.DbNull);
            _readerForMethod.Verify();
        }
    }
}
