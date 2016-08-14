using System;
using System.Collections.Generic;
using System.Text;

namespace DaoUtilsCore.core
{
    public class DataTypeAdaptor
    {
        public static T ConvertValue<T>(object value, T defaultValue = default(T))
        {
            if (value == null) return defaultValue;
            var fromType = value.GetType();
            var toType = typeof (T);
            if (toType.IsAssignableFrom(fromType)) return (T)value;
            if (typeof (IConvertible).IsAssignableFrom(fromType) && typeof (IConvertible).IsAssignableFrom(toType))
            {
                return (T)Convert.ChangeType(value, toType);
            }
            throw new DaoUtilsException($"Don't know how to convert'{fromType}' to {toType}"); ;
        }

    }
}
