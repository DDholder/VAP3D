using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSUIPC;

namespace VAP3D
{
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

        public bool WriteOnly
        {
            get { return m_wrappedOffset.WriteOnly; }
            set { m_wrappedOffset.WriteOnly = value; }
        }

        public bool IsConnected
        {
            get { return m_wrappedOffset.IsConnected; }
        }

        public int Address
        {
            get { return m_wrappedOffset.Address; }
            set { m_wrappedOffset.Address = value; }
        }

        public void Disconnect(bool AfterNextProcess)
        {
            m_wrappedOffset.Disconnect(AfterNextProcess);
        }

        public void Disconnect()
        {
            m_wrappedOffset.Disconnect();
        }

        public void Reconnect(bool ForNextProcessOnly)
        {
            m_wrappedOffset.Reconnect(ForNextProcessOnly);
        }

        public void Reconnect()
        {
            m_wrappedOffset.Reconnect();
        }

        public override string ToString()
        {
            return m_wrappedOffset.ToString();
        }

        public Type GetUnderlyingType()
        {
            return typeof(T);
        }

        public T GetValue()
        {
            return m_wrappedOffset.Value;
        }

        public void SetValue(T value)
        {
            m_wrappedOffset.Value = value;
        }
    }
}
