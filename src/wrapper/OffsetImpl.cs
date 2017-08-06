using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSUIPC;

namespace VAP3D
{
    public class OffsetImpl : IOffset
    {
        private Offset m_wrappedOffset = null;

        public OffsetImpl(int Address, int Length)
        {
            m_wrappedOffset = new Offset(Address, Length);
        }

        public OffsetImpl(int Address, int Length, bool WriteOnly)
        {
            m_wrappedOffset = new Offset(Address, Length, WriteOnly);
        }

        public OffsetImpl(string DataGroupName, int Address, int Length)
        {
            m_wrappedOffset = new Offset(DataGroupName, Address, Length);
        }

        public OffsetImpl(string DataGroupName, int Address, int Length, bool WriteOnly)
        {
            m_wrappedOffset = new Offset(DataGroupName, Address, Length, WriteOnly);
        }

        public bool IsFixedLengthString
        {
            get { return m_wrappedOffset.IsFixedLengthString; }
            set { m_wrappedOffset.IsFixedLengthString = value; }
        }

        public bool WriteOnly
        {
            get { return m_wrappedOffset.WriteOnly; }
            set { m_wrappedOffset.WriteOnly = value; }
        }

        public bool IsConnected
        {
            get { return m_wrappedOffset.IsConnected; }
        }

        public OffsetAction ActionAtNextProcess
        {
            get { return m_wrappedOffset.ActionAtNextProcess; }
            set { m_wrappedOffset.ActionAtNextProcess = value; }
        }

        public int Address
        {
            get { return m_wrappedOffset.Address; }
            set { m_wrappedOffset.Address = value; }
        }

        public Guid ID
        {
            get { return m_wrappedOffset.ID;  }
        }

        public int DataLength { get; }

        public void Disconnect(bool AfterNextProcess)
        {
            m_wrappedOffset.Disconnect(AfterNextProcess);
        }

        public void Disconnect()
        {
            m_wrappedOffset.Disconnect();
        }

        public T GetValue<T>()
        {
            return m_wrappedOffset.GetValue<T>();
        }

        public object GetValue(Type AsType)
        {
            return m_wrappedOffset.GetValue(AsType);
        }

        public void Reconnect(bool ForNextProcessOnly)
        {
            m_wrappedOffset.Reconnect(ForNextProcessOnly);
        }

        public void Reconnect()
        {
            m_wrappedOffset.Reconnect();
        }

        public void SetValue(object NewValue)
        {
            m_wrappedOffset.SetValue(NewValue);
        }

        public override string ToString()
        {
            return m_wrappedOffset.ToString();
        }
    }

    public class GenericOffsetImpl<T> : IOffset<T>
    {
        private Offset<T> m_wrappedOffset = null;

        public GenericOffsetImpl(int Address)
        {
            m_wrappedOffset = new Offset<T>(Address);
        }

        public GenericOffsetImpl(string DataGroupName, int Address)
        {
            m_wrappedOffset = new Offset<T>(DataGroupName, Address);
        }

        public GenericOffsetImpl(int Address, int ArrayOrStringLength)
        {
            m_wrappedOffset = new Offset<T>(Address, ArrayOrStringLength);
        }

        public GenericOffsetImpl(int Address, bool WriteOnly)
        {
            m_wrappedOffset = new Offset<T>(Address, WriteOnly);
        }

        public GenericOffsetImpl(string DataGroupName, int Address, bool WriteOnly)
        {
            m_wrappedOffset = new Offset<T>(DataGroupName, Address, WriteOnly);
        }

        public GenericOffsetImpl(int Address, int ArrayOrStringLength, bool WriteOnly)
        {
            m_wrappedOffset = new Offset<T>(Address, ArrayOrStringLength, WriteOnly);
        }

        public GenericOffsetImpl(string DataGroupName, int Address, int ArrayOrStringLength)
        {
            m_wrappedOffset = new Offset<T>(DataGroupName, Address, ArrayOrStringLength);
        }

        public GenericOffsetImpl(string DataGroupName, int Address, int ArrayOrStringLength, bool WriteOnly)
        {
            m_wrappedOffset = new Offset<T>(DataGroupName, Address, ArrayOrStringLength, WriteOnly);
        }

        public T Value
        {
            get { return m_wrappedOffset.Value; }
            set { m_wrappedOffset.Value = value; }
        }

        public bool IsFixedLengthString
        {
            get { return m_wrappedOffset.IsFixedLengthString; }
            set { m_wrappedOffset.IsFixedLengthString = value; }
        }

        public bool WriteOnly
        {
            get { return m_wrappedOffset.WriteOnly; }
            set { m_wrappedOffset.WriteOnly = value; }
        }

        public bool IsConnected
        {
            get { return m_wrappedOffset.IsConnected; }
        }

        public OffsetAction ActionAtNextProcess
        {
            get { return m_wrappedOffset.ActionAtNextProcess; }
            set { m_wrappedOffset.ActionAtNextProcess = value; }
        }

        public int Address
        {
            get { return m_wrappedOffset.Address; }
            set { m_wrappedOffset.Address = value; }
        }

        public Guid ID
        {
            get { return m_wrappedOffset.ID; }
        }

        public int DataLength { get; }

        public void Disconnect(bool AfterNextProcess)
        {
            m_wrappedOffset.Disconnect(AfterNextProcess);
        }

        public void Disconnect()
        {
            m_wrappedOffset.Disconnect();
        }

        public TType GetValue<TType>()
        {
            return m_wrappedOffset.GetValue<TType>();
        }

        public object GetValue(Type AsType)
        {
            return m_wrappedOffset.GetValue(AsType);
        }

        public void Reconnect(bool ForNextProcessOnly)
        {
            m_wrappedOffset.Reconnect(ForNextProcessOnly);
        }

        public void Reconnect()
        {
            m_wrappedOffset.Reconnect();
        }

        public void SetValue(object NewValue)
        {
            m_wrappedOffset.SetValue(NewValue);
        }

        public override string ToString()
        {
            return m_wrappedOffset.ToString();
        }
    }
}
