using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAP3D
{
    public interface IOffset
    {
        //
        // Summary:
        //     True is this offset represents a fixed length string.
        bool IsFixedLengthString { get; set; }
        //
        // Summary:
        //     Indicates that this Offset is to be Write-Only. While marked as Write-Only the
        //     value of this Offset will never be read from FSUIPC. If you change the value
        //     of this Offset the new value will be written to FSUIPC on the next process.
        bool WriteOnly { get; set; }
        //
        // Summary:
        //     Indicates if this Offset is currently 'Connected'. Offsets that are disconnected
        //     will not be updated or have changes written to FSUIPC when Process() is run.
        //     This is read-only. To connect or disconnect an Offset call the Connect() or Disconnect()
        //     methods.
        bool IsConnected { get; }
        //
        // Summary:
        //     This property can be used to force this offset to be read or written on the next
        //     process.
        //     Normally you need not set this as the default (AutoSense) works perfectly well.
        FSUIPC.OffsetAction ActionAtNextProcess { get; set; }
        //
        // Summary:
        //     The FSUIPC offset address to read from or write to, as specified in the FSUIPC
        //     for Programmer's document.
        int Address { get; set; }
        //
        // Summary:
        //     The unique internal ID of this offset
        Guid ID { get; }
        //
        // Summary:
        //     The length of the data in this offset in bytes.
        int DataLength { get; }

        //
        // Summary:
        //     Disconnects this Offset from the FSUIPCConnection class. The value of this Offset
        //     will no longer be updated or written during Process() calls. Use Reconnect()
        //     to start updating again.
        //
        // Parameters:
        //   AfterNextProcess:
        //     If true, the Offset will not be disconnected until after you call Process().
        //     If false the Offset is disconnected immediately.
        void Disconnect(bool AfterNextProcess);
        //
        // Summary:
        //     Immediately disconnects this Offset from FSUIPC. The value of this Offset will
        //     no longer be updated or written during Process() calls. Use Reconnect() to start
        //     updating again.
        void Disconnect();
        //
        // Summary:
        //     Gets the value of this offset as the specified type
        //
        // Type parameters:
        //   T:
        //     The type the value should be returned as
        //
        // Returns:
        //     The value of the offset
        T GetValue<T>();
        //
        // Summary:
        //     Gets the value of this offset as the specified type
        //
        // Parameters:
        //   AsType:
        //     The type the value should be returned as
        //
        // Returns:
        //     The value of the offset
        object GetValue(Type AsType);
        //
        // Summary:
        //     Reconnects this Offset to FSUIPC. The value of this Offset will be updated/written
        //     during subsequent Process() calls.
        //
        // Parameters:
        //   ForNextProcessOnly:
        //     If true, the Offset is reconnected only for the next Process() call. After that
        //     it's disconnected again. If false, the Offset is reconnected until you call Disconnect().
        void Reconnect(bool ForNextProcessOnly);
        //
        // Summary:
        //     Permenently reconnects this Offset to FSUIPC. The value of this Offset will be
        //     read/written during subsequent Process() calls.
        void Reconnect();
        //
        // Summary:
        //     Sets the value of this offset. No type checking or other data varification is
        //     done.
        //
        // Parameters:
        //   NewValue:
        void SetValue(object NewValue);
        //
        // Summary:
        //     Return a string value contianing information about this Offset.
        //
        // Returns:
        //     A string containing the Offset Address and Length
        string ToString();
    }

    public interface IOffset<T> : IOffset
    {
        T Value { get; set; }
    }
}
