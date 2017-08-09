using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    class DefaultOffsetFactory : IOffsetFactory
    {
        public IOffset<T> createOffset<T>(int Address)
        {
            return new GenericOffsetImpl<T>(Address);
        }

        public IOffset<T> createOffset<T>(string DataGroupName, int Address)
        {
            return new GenericOffsetImpl<T>(DataGroupName, Address);
        }

        public IOffset<T> createOffset<T>(int Address, int ArrayOrStringLength)
        {
            return new GenericOffsetImpl<T>(Address, ArrayOrStringLength);
        }

        public IOffset<T> createOffset<T>(int Address, bool WriteOnly)
        {
            return new GenericOffsetImpl<T>(Address, WriteOnly);
        }

        public IOffset<T> createOffset<T>(string DataGroupName, int Address, bool WriteOnly)
        {
            return new GenericOffsetImpl<T>(DataGroupName, Address, WriteOnly);
        }

        public IOffset<T> createOffset<T>(int Address, int ArrayOrStringLength, bool WriteOnly)
        {
            return new GenericOffsetImpl<T>(Address, ArrayOrStringLength, WriteOnly);
        }

        public IOffset<T> createOffset<T>(string DataGroupName, int Address, int ArrayOrStringLength)
        {
            return new GenericOffsetImpl<T>(DataGroupName, Address, ArrayOrStringLength);
        }

        public IOffset<T> createOffset<T>(string DataGroupName, int Address, int ArrayOrStringLength, bool WriteOnly)
        {
            return new GenericOffsetImpl<T>(DataGroupName, Address, ArrayOrStringLength, WriteOnly);
        }

        public IOffset createOffsetForType(int Address, Type t)
        {
            return createOffsetForType(Address, t, false);
        }

        public IOffset createOffsetForType(int Address, Type t, bool writeOnly)
        {
            if (t == typeof(Char))
                return createOffset<char>(Address, writeOnly);
            else if (t == typeof(Byte))
                return createOffset<byte>(Address, writeOnly);
            else if (t == typeof(Int16))
                return createOffset<short>(Address, writeOnly);
            else if (t == typeof(UInt16))
                return createOffset<ushort>(Address, writeOnly);
            else if (t == typeof(Int32))
                return createOffset<int>(Address, writeOnly);
            else if (t == typeof(UInt32))
                return createOffset<uint>(Address, writeOnly);
            else if (t == typeof(Int64))
                return createOffset<long>(Address, writeOnly);
            else if (t == typeof(UInt64))
                return createOffset<ulong>(Address, writeOnly);
            else if (t == typeof(Single))
                return createOffset<float>(Address, writeOnly);
            else if (t == typeof(Double))
                return createOffset<double>(Address, writeOnly);
            else if (t == typeof(Boolean))
                return createOffset<bool>(Address, writeOnly);

            return null;
        }
    }
}
