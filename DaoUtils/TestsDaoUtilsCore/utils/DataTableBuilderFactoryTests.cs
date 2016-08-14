using System.Data;
using DaoUtilsCore.def.utils;
using DaoUtilsCore.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsDaoUtilsCore.utils
{
    [TestClass]
    public class DataTableBuilderFactoryTests
    {
        [TestMethod]
        public void TableBuilder()
        {
            const string tableName = "A Table";
            const string columnName = "A Column";
            IDataTableBuilder builder = DataTableBuilderFactory.TableBuilder(tableName);
            builder.Int32Column(columnName);
            DataTable table = builder.BuildTable();
            Assert.IsNotNull(table);
            Assert.AreEqual(tableName, table.TableName);
            Assert.AreEqual(1, table.Columns.Count);
            Assert.AreEqual(columnName, table.Columns[0].ColumnName);
            Assert.AreEqual(0, table.Rows.Count);
        }
    }
}
