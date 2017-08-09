using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class Utilities
    {
        private static T ConvertValue<T, U>(U value) where U : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

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

        public static bool setVariableValue(Type dataType, object value, string destination, dynamic vaProxy)
        {
            if (dataType == typeof(float) || dataType == typeof(double))
            {
                vaProxy.SetDecimal(destination, Convert.ToDecimal(value));
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
                vaProxy.SetBoolean(destination, Convert.ToBoolean(value));
            }
            else
            {
                return false; // Unspported data type
            }

            return true;
        }

        public static bool setVariableValueFromOffset<T>(Type dataType, IOffset<T> offset, string destination, dynamic vaProxy)
        {
            return setVariableValue(dataType, offset.Value, destination, vaProxy);
        }

        public static object getVariableValue(Type dataType, string source, dynamic vaProxy)
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

        public static object getOffsetValue(IOffset offset)
        {
            Type t = offset.GetUnderlyingType();

            if (t == typeof(Char))
                return ((IOffset<char>)offset).GetValue();
            else if (t == typeof(Byte))
                return ((IOffset<byte>)offset).GetValue();
            else if (t == typeof(Int16))
                return ((IOffset<short>)offset).GetValue();
            else if (t == typeof(UInt16))
                return ((IOffset<ushort>)offset).GetValue();
            else if (t == typeof(Int32))
                return ((IOffset<int>)offset).GetValue();
            else if (t == typeof(UInt32))
                return ((IOffset<uint>)offset).GetValue();
            else if (t == typeof(Int64))
                return ((IOffset<long>)offset).GetValue();
            else if (t == typeof(UInt64))
                return ((IOffset<ulong>)offset).GetValue();
            else if (t == typeof(Single))
                return ((IOffset<float>)offset).GetValue();
            else if (t == typeof(Double))
                return ((IOffset<double>)offset).GetValue();
            else if (t == typeof(Boolean))
                return ((IOffset<bool>)offset).GetValue();

            return null;
        }

        public static void setOffsetValue(IOffset offset, object value)
        {
            Type t = offset.GetUnderlyingType();

            if (t == typeof(Char))
            {
                IOffset<char> off = (IOffset<char>)offset;
                off.SetValue(Convert.ToChar(value));
            }
            else if (t == typeof(Byte))
            {
                IOffset<byte> off = (IOffset<byte>)offset;
                off.SetValue(Convert.ToByte(value));
            }
            else if (t == typeof(Int16))
            {
                IOffset<short> off = (IOffset<short>)offset;
                off.SetValue(Convert.ToInt16(value));
            }
            else if (t == typeof(UInt16))
            {
                IOffset<ushort> off = (IOffset<ushort>)offset;
                off.SetValue(Convert.ToUInt16(value));
            }
            else if (t == typeof(Int32))
            {
                IOffset<int> off = (IOffset<int>)offset;
                off.SetValue(Convert.ToInt32(value));
            }
            else if (t == typeof(UInt32))
            {
                IOffset<uint> off = (IOffset<uint>)offset;
                off.SetValue(Convert.ToUInt32(value));
            }
            else if (t == typeof(Int64))
            {
                IOffset<long> off = (IOffset<long>)offset;
                off.SetValue(Convert.ToInt64(value));
            }
            else if (t == typeof(UInt64))
            {
                IOffset<ulong> off = (IOffset<ulong>)offset;
                off.SetValue(Convert.ToUInt64(value));
            }
            else if (t == typeof(Single))
            {
                IOffset<float> off = (IOffset<float>)offset;
                off.SetValue(Convert.ToSingle(value));
            }
            else if (t == typeof(Double))
            {
                IOffset<double> off = (IOffset<double>)offset;
                off.SetValue(Convert.ToDouble(value));
            }
            else if (t == typeof(Boolean))
            {
                IOffset<bool> off = (IOffset<bool>)offset;
                off.SetValue(Convert.ToBoolean(value));
            }
        }
    }
}
