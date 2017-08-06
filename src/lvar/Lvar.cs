using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public class Lvar
    {
        protected enum SizeMask
        {
            DOUBLE = 0x00000,
            FLOAT = 0x10000,
            INT32 = 0x20000,
            UINT32 = 0x30000,
            INT16 = 0x40000,
            UINT16 = 0x50000,
            INT8 = 0x60000,
            UINT8 = 0x70000,

            INVALID = 0xFFFFFF
        }

        protected SizeMask geSizeMaskForType(Type type)
        {
            if (type == typeof(Char))
                return SizeMask.INT8;
            else if (type == typeof(Byte) || type == typeof(Boolean))
                return SizeMask.UINT8;
            else if (type == typeof(Int16))
                return SizeMask.INT16;
            else if (type == typeof(UInt16))
                return SizeMask.UINT16;
            else if (type == typeof(Int32))
                return SizeMask.INT32;
            else if (type == typeof(UInt32))
                return SizeMask.UINT32;
            else if (type == typeof(Single))
                return SizeMask.FLOAT;
            else if (type == typeof(Double))
                return SizeMask.DOUBLE;

            return SizeMask.INVALID;
        }
    }
}
