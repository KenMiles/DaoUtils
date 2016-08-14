using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DaoUtils.core;
using DaoUtilsCore.def.utils;
using DaoUtilsCore.def.ms;

namespace DaoUtilsCore.ms
{
    class DaoStructuredDataParameter : DaoParameter<DataTable>, IDaoStructuredDataParameterInput, IDaoStructuredDataParameterInternal
    {
        public DaoStructuredDataParameter(SqlParameter dbDataParameter, string name, ParameterDirection direction, int size) : base(dbDataParameter, name, direction, size)
        {
        }

        private bool _reset = false;
        void IDaoStructuredDataParameterInternal.AddValue(DataTable table)
        {
            if (_reset || InputValues == null)
            {
                SetValues(new[] {table});
                _reset = false;
            }
            else
            {
                InputValues.Add(table);
            }
        }

        public IParameterDataTableBuilder TableBuilder(string tableName = null)
        {
            return new ParameterDataTableBuilder(string.IsNullOrWhiteSpace(tableName) ? $"{Name}-{Parameter}" : tableName, this);
        }

        public override void PostCall()
        {
            base.PostCall();
            _reset = true;
        }

        public new DaoStructuredDataParameter SetValue(DataTable value)
        {
            base.SetValue(value);
            return this;
        }

        public new DaoStructuredDataParameter SetValues(IEnumerable<DataTable> values)
        {
            base.SetValues(values);
            return this;
        }

    }
}
