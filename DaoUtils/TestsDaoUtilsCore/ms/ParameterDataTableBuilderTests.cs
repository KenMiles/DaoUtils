using System.Collections.Generic;
using System.Data;
using System.Linq;
using DaoUtils.core;
using DaoUtilsCore.def.ms;
using DaoUtilsCore.ms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.ms
{
    [TestClass]
    public class ParameterDataTableBuilderTests
    {
        private void CheckTable(DataTable table)
        {
            Assert.IsNotNull(table);
            Assert.AreEqual("A Table", table.TableName);
            Assert.AreEqual(1, table.Columns.Count);
            Assert.AreEqual("A Column", table.Columns[0].ColumnName);
            Assert.AreEqual(string.Join("\r\n", new[] { "One", "Two", "Three" }), 
                string.Join("\r\n", table.Rows.Cast<DataRow>().Select(r => r[0])));
        }

        private readonly Mock<IDaoStructuredDataParameterInternal> _parameter = new Mock<IDaoStructuredDataParameterInternal>(MockBehavior.Strict);
        private ParameterDataTableBuilder _uut;

        [TestInitialize]
        public void SetUp()
        {
            _uut = new ParameterDataTableBuilder("A Table", _parameter.Object);
            _uut.Column("A Column", new[] {"One", "Two", "Three"});
        }

        [TestMethod]
        public void BuildAndSetAsValue()
        {
            var tables = new List<DataTable>();
            _parameter.Setup(p => p.SetValue(It.IsAny<DataTable>())).Returns((DaoParameter < DataTable >)null).Callback<DataTable>(dt => tables.Add(dt));
            _uut.BuildAndSetAsValue();
            Assert.AreEqual(1, tables.Count);
            CheckTable(tables[0]);
        }

        [TestMethod]
        public void BuildAndAddTable()
        {
            var tables = new List<DataTable>();
            _parameter.Setup(p => p.AddValue(It.IsAny<DataTable>())).Callback<DataTable>(dt => tables.Add(dt));
            Assert.AreEqual(_uut, _uut.BuildAndAddTable());
            Assert.AreEqual(1, tables.Count);
            CheckTable(tables[0]);
        }
    }
}
