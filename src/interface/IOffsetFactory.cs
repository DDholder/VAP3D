using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public interface IOffsetFactory
    {
        //
        // Summary:
        //     Creates a Offset that will read from or write the specified number of bytes to
        //     the specified Offset. Only used with data types of String, BitArray and an array
        //     of bytes.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   Length:
        //     The number of bytes to read/write.
        IOffset createOffset(int Address, int Length);

        //
        // Summary:
        //     Creates a new Offset that will read from or write the specified number of bytes
        //     to the specified Offset. Only used with data types of String, BitArray and an
        //     array of bytes.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   Length:
        //     The number of bytes to read/write.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset createOffset(int Address, int Length, bool WriteOnly);

        //
        // Summary:
        //     Creates a new Offset that will read from or write the specified number of bytes
        //     to the specified Offset. Only used with data types of String, BitArray and an
        //     array of bytes.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   Length:
        //     The number of bytes to read/write.
        IOffset createOffset(string DataGroupName, int Address, int Length);

        //
        // Summary:
        //     Creates a new Offset in the specified group that will read from or write the
        //     specified number of bytes to the specified Offset. Only used with data types
        //     of String, BitArray and an array of bytes.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   Length:
        //     The number of bytes to read/write.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset createOffset(string DataGroupName, int Address, int Length, bool WriteOnly);

        //
        // Summary:
        //     Creates a new Offset that will read from or write to the specified Offset.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        IOffset<T> createOffset<T>(int Address);
        //
        // Summary:
        //     Creates a new Offset in the specified Group that will read from or write to the
        //     specified Offset.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        IOffset<T> createOffset<T>(string DataGroupName, int Address);
        //
        // Summary:
        //     Creates a Offset that will read from or write the specified number of bytes to
        //     the specified Offset. Only used with data types of String, BitArray, FsLongitude,
        //     FsLatitude and an array of bytes.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   ArrayOrStringLength:
        //     The number of bytes to read. Used to define the length of types: String, BitArray,
        //     FsLongitude, FsLatitude and array of bytes. Use a negative length for strings
        //     without 0 terminators.
        IOffset<T> createOffset<T>(int Address, int ArrayOrStringLength);
        //
        // Summary:
        //     Creates a new Offset that will read from or write to the specified Offset.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset<T> createOffset<T>(int Address, bool WriteOnly);
        //
        // Summary:
        //     Creates a new Offset in the specified Group that will read from or write to the
        //     specified Offset.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset<T> createOffset<T>(string DataGroupName, int Address, bool WriteOnly);
        //
        // Summary:
        //     Creates a new Offset that will read from or write the specified number of bytes
        //     to the specified Offset. Only used with data types of String, BitArray, FsLongitude,
        //     FsLatitude and an array of bytes.
        //
        // Parameters:
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   ArrayOrStringLength:
        //     The number of bytes to read. Used to define the length of types: String, BitArray,
        //     FsLongitude, FsLatitude and array of bytes. Use a negative length for strings
        //     without 0 terminators.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset<T> createOffset<T>(int Address, int ArrayOrStringLength, bool WriteOnly);
        //
        // Summary:
        //     Creates a new Offset that will read from or write the specified number of bytes
        //     to the specified Offset. Only used with data types of String, BitArray, FsLongitude,
        //     FsLatitude and an array of bytes.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   ArrayOrStringLength:
        //     The number of bytes to read. Used to define the length of types: String, BitArray,
        //     FsLongitude, FsLatitude and array of bytes. Use a negative length for strings
        //     without 0 terminators.
        IOffset<T> createOffset<T>(string DataGroupName, int Address, int ArrayOrStringLength);
        //
        // Summary:
        //     Creates a new Offset in the specified group that will read from or write the
        //     specified number of bytes to the specified Offset. Only used with data types
        //     of String, BitArray, FsLongitude, FsLatitude and an array of bytes.
        //
        // Parameters:
        //   DataGroupName:
        //     The name of the DataGroup to put this Offset into.
        //     To process this Offset you must call Process() and pass the GroupName as a parameter.
        //
        //   Address:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        //
        //   ArrayOrStringLength:
        //     The number of bytes to read. Used to define the length of types: String, BitArray,
        //     FsLongitude, FsLatitude and array of bytes. Use a negative length for strings
        //     without 0 terminators.
        //
        //   WriteOnly:
        //     If true, Sets this Offset to only write data. Its value is never read from FSUIPC.
        //     You can change the setting with the WriteOnly property at any time.
        IOffset<T> createOffset<T>(string DataGroupName, int Address, int ArrayOrStringLength, bool WriteOnly);
    }
}
