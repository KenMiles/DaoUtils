using System;
using System.Data;
using System.Linq;
using System.Threading;
using Common.Logging;
using DaoUtils.core;
using DaoUtils.Standard;
using DaoUtilsCore.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.core
{
    internal delegate IReadValue DoCreateColumnReader(IDataReader reader, int columnIndex);
    internal class TestableDataReaderHelperAbstract : DataReaderHelperAbstract<IReadValue>
    {
        public TestableDataReaderHelperAbstract(IDataReader reader, ILog log,  DoCreateColumnReader createColumnReader) : base(reader, log)
        {
            _createColumn = createColumnReader;
        }

        private readonly DoCreateColumnReader _createColumn;
        protected override IReadValue CreateColumnReader(IDataReader reader, int columnIndex)
        {
            return _createColumn(reader, columnIndex);
        }
    }

    [TestClass]
    public class DataReaderHelperAbstractTests
    {
        private static readonly string[] ColumnNames = {"Zero", "One", "Two", "Three", "Four", "Five"};
        private readonly Mock<IReadValue>[] _readColumns = ColumnNames.Select(n => new Mock<IReadValue>(MockBehavior.Strict)).ToArray();
        private readonly Mock<ILog> _log = new Mock<ILog>(MockBehavior.Loose);
        private readonly Mock<IDataReader> _reader = new Mock<IDataReader>(MockBehavior.Strict);
        private TestableDataReaderHelperAbstract _uut;

        private IReadValue CreateColumnReader(IDataReader reader, int columnIndex)
        {
            Assert.AreEqual(_reader.Object, reader);
            return _readColumns[columnIndex].Object;
        }

        [TestInitialize]
        public void Setup()
        {
            _uut = new TestableDataReaderHelperAbstract(_reader.Object, _log.Object, CreateColumnReader);
            _reader.Setup(r => r.FieldCount).Returns(ColumnNames.Length);
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                var i1 = i;
                _reader.Setup(r => r.GetName(i1)).Returns(ColumnNames[i]);
            }
        }

        [TestMethod]
        public void IndexColumns()
        {
            var uutAs = _uut as IReadHelper<IReadValue>;
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                Assert.AreEqual(_readColumns[i].Object, uutAs[i]);
            }
        }

        [TestMethod]
        public void NamedColumns()
        {
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                Assert.AreEqual(_readColumns[i].Object, _uut[ColumnNames[i]]);
                Assert.AreEqual(_readColumns[i].Object, _uut[ColumnNames[i].ToLower()]);
                Assert.AreEqual(_readColumns[i].Object, _uut[ColumnNames[i].ToUpper()]);

                Assert.AreEqual(_readColumns[i].Object, _uut.Named(ColumnNames[i]));
                Assert.AreEqual(_readColumns[i].Object, _uut.Named(ColumnNames[i].ToLower()));
                Assert.AreEqual(_readColumns[i].Object, _uut.Named(ColumnNames[i].ToUpper()));
            }
        }

        [TestMethod]
        public void UnknownColumnName()
        {
            try
            {
                _uut.Named("OneHundred");
                Assert.Fail("Exception Expected");
            }
            catch (DaoUtilsException ex)
            {
                Assert.AreEqual($"Unknown Column '{"OneHundred"}'", ex.Message);
            }
            _log.Verify(l => l.Error($"Unknown Column '{"OneHundred"}'"));
        }

        [TestMethod]
        public void NullColumnName()
        {
            try
            {
                _uut.Named(null);
                Assert.Fail("Exception Expected");
            }
            catch (DaoUtilsException ex)
            {
                Assert.AreEqual($"Unknown Column '{null}'", ex.Message);
            }
            _log.Verify(l => l.Error($"Unknown Column '{null}'"));
        }
    }
}
