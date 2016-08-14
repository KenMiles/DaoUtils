using System.Data;
using DaoUtils.core;
using DaoUtils.Standard;

namespace DaoUtils.Standard
{
    internal class DataReaderHelper : DataReaderHelperAbstract<IReadValue>
    {
        public DataReaderHelper(IDataReader reader) : base(reader)
        {
        }

        protected override IReadValue CreateColumnReader(IDataReader reader, int columnIndex)
        {
            return new ColumnReadValue(reader, columnIndex);
        }
    }

}
