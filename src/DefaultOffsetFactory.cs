using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    class DefaultOffsetFactory : IOffsetFactory
    {
        public IOffset createOffset(int Address, int Length)
        {
            return new OffsetImpl(Address, Length);
        }

        public IOffset createOffset(int Address, int Length, bool WriteOnly)
        {
            return new OffsetImpl(Address, Length, WriteOnly);
        }

        public IOffset createOffset(string DataGroupName, int Address, int Length)
        {
            return new OffsetImpl(DataGroupName, Address, Length);
        }

        public IOffset createOffset(string DataGroupName, int Address, int Length, bool WriteOnly)
        {
            return new OffsetImpl(DataGroupName, Address, Length, WriteOnly);
        }

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
    }
}
