using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DaoUtilsCore.def.ms;
using DaoUtilsCore.ms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TestsDaoUtilsCore.ms
{
    internal class AccessDaoStructuredDataParameter : DaoStructuredDataParameter
    {
        public AccessDaoStructuredDataParameter(SqlParameter dbDataParameter, string name, ParameterDirection direction, int size) : base(dbDataParameter, name, direction, size)
        {
        }

        public List<DataTable> AccessInputValues => InputValues;
        public List<DataTable> AccessOutputValues => Values();
    }

    [TestClass]
    public class DaoStructuredDataParameterTests
    {
        private readonly SqlParameter _parameter = new SqlParameter();
        private AccessDaoStructuredDataParameter _uut;

        [TestInitialize]
        public void SetUp()
        {
            // Note Only ever input
            _uut = new AccessDaoStructuredDataParameter(_parameter, "A Parameter", ParameterDirection.Input, 0);
        }

        private void CheckTable(DataTable table, string tableName, string columnName, params string[] rowValues)
        {
            Assert.IsNotNull(table);
            Assert.AreEqual(tableName, table.TableName);
            Assert.AreEqual(1, table.Columns.Count);
            Assert.AreEqual(columnName, table.Columns[0].ColumnName);
            Assert.AreEqual(string.Join("\r\n", rowValues), string.Join("\r\n", table.Rows.Cast<DataRow>().Select(r => r[0])));
        }

        private void CheckTable(int tableNo, string tableName, string columnName, params string[] rowValues)
        {
            Assert.IsNotNull(_uut.AccessInputValues);
            CheckTable(_uut.AccessInputValues[tableNo], tableName, columnName, rowValues);
        }

        [TestMethod]
        public void AsInputIfTableBuilderAddsValues()
        {
            IDaoStructuredDataParameterInput uut = _uut;
            var builder = uut.TableBuilder("A Builder Table");
            var column = builder.StringColumn("A Column");
            column.Values = new[] {"One", "Two", "Three"};
            builder.BuildAndAddTable();
            column.Values = new[] { "Four", "Five", "Six", "Seven" };
            builder.BuildAndAddTable();
            Assert.AreEqual(2, _uut.AccessInputValues?.Count);
            CheckTable(0, "A Builder Table", "A Column", "One", "Two", "Three");
            CheckTable(1, "A Builder Table", "A Column", "Four", "Five", "Six", "Seven");

            //This should setup Reset Flag, but will only reset when new values added
            _uut.PostCall();
            column.Values = new[] { "Eight", "Nine" };
            builder.BuildAndAddTable();
            Assert.AreEqual(1, _uut.AccessInputValues?.Count);
            CheckTable(0, "A Builder Table", "A Column", "Eight", "Nine");
            //This checks PostCall called base method (wouldn't be called if direction is input, but i/f doesn't care about direction
            Assert.AreEqual(0, _uut.OutputParamArraySize);
        }

        [TestMethod]
        public void AsInputBuildAndSetAsValue()
        {
            IDaoStructuredDataParameterInput uut = _uut;
            var builder = uut.TableBuilder("A Builder Table");
            var column = builder.StringColumn("A Column");
            column.Values = new[] { "One", "Two", "Three" };
            builder.BuildAndSetAsValue();
            CheckTable((DataTable)_parameter.Value, "A Builder Table", "A Column", "One", "Two", "Three");
        }

        [TestMethod]
        public void AsInputBuildAndSetAsValueBatch()
        {
            IDaoStructuredDataParameterInput uut = _uut;
            var builder = uut.TableBuilder("A Builder Table");
            var column = builder.StringColumn("A Column");
            column.Values = new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };
            builder.BuildAndSetAsValue(4);
            Assert.AreEqual(3, _uut.AccessInputValues?.Count);
            CheckTable(0, "A Builder Table", "A Column", "One", "Two", "Three", "Four");
            CheckTable(1, "A Builder Table", "A Column", "Five", "Six", "Seven", "Eight");
            CheckTable(2, "A Builder Table", "A Column", "Nine", "Ten");
        }
    }
}
