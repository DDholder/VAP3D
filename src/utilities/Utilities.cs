using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class Utilities
    {
        public static int numBytesFromType(Type type)
        {
            if (type == typeof(Char) || type == typeof(Byte))
                return 1;
            else if (type == typeof(Int16) || type == typeof(UInt16))
                return 2;
            else if (type == typeof(Int32) || type == typeof(UInt32))
                return 4;
            else if (type == typeof(Int64) || type == typeof(UInt64))
                return 8;
            else if (type == typeof(Single))
                return 4;
            else if (type == typeof(Double))
                return 8;
            else if (type == typeof(Boolean))
                return 1;

            return -1;
        }

        public static bool setVariableValueFromOffset(Type dataType, IOffset offset, string destination, dynamic vaProxy)
        {
            object value = offset.GetValue(dataType);

            if (dataType == typeof(float) || dataType == typeof(double))
            {
                vaProxy.SetDecimal(destination, value);
            }
            else if (dataType == typeof(int) || dataType == typeof(uint) ||
                dataType == typeof(short) || dataType == typeof(ushort) ||
                dataType == typeof(char) || dataType == typeof(byte) ||
                dataType == typeof(long) || dataType == typeof(ulong))
            {
                vaProxy.SetInt(destination, Convert.ToInt32(value));
            }
            else if (dataType == typeof(bool))
            {
                vaProxy.SetBoolean(destination, value);
            }
            else
            {
                return false; // Unspported data type
            }

            return true;
        }

        public static object getVariableValueForOffset(Type dataType, string source, dynamic vaProxy)
        {
            if (dataType == typeof(float) || dataType == typeof(double))
            {
                return vaProxy.GetDecimal(source);
            }
            else if (dataType == typeof(int) || dataType == typeof(uint) ||
                dataType == typeof(short) || dataType == typeof(ushort) ||
                dataType == typeof(char) || dataType == typeof(byte) ||
                dataType == typeof(long) || dataType == typeof(ulong))
            {
                return vaProxy.GetInt(source);
            }
            else if (dataType == typeof(bool))
            {
                return vaProxy.GetBoolean(source);
            }
            else
            {
                return null; // Unspported data type
            }
        }
    }
}
