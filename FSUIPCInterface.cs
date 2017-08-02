using System;
using System.Threading;
using System.Collections.Generic;
using FSUIPC;

namespace VAP3D
{
    public class FSUIPCInterface
    {
        private static int CONNECT_SLEEP_TIME = 1000;
        private static int EVENT_LOOP_DELAY = 10000;

        private dynamic m_vaProxy;

        private bool m_isConnected;

        private Thread m_connectionThread;
        private Thread m_eventMonitorThread;

        // -----------------------------------------
        // Private methods
        private bool tryConnect()
        {
            try
            {
                FSUIPCConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void connectionRunner()
        {
            try
            {
                while (!m_isConnected)
                {
                    if (tryConnect())
                    {
                        m_isConnected = true;
                        m_vaProxy.WriteToLog("Connected to FSUIPC", "green");
                    }
                    else
                    {
                        Thread.Sleep(CONNECT_SLEEP_TIME);
                    }
                }
            }
            catch(ThreadAbortException e)
            {
            }
        }

        private void process()
        {
            FSUIPCConnection.Process();
        }

        private void writeErrorToLog(string error)
        {
            m_vaProxy.WriteToLog("VA:P3D Error: " + error, "red");
        }

        private void eventMonitorRunner()
        {

        }

        // Public Interface
        public void beginMonitoringEvents()
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }

            m_eventMonitorThread = new Thread(eventMonitorRunner);
            m_eventMonitorThread.Start();
        }

        public void stopMonitoringEvents()
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }

            m_eventMonitorThread.Abort();
            m_eventMonitorThread.Join();

            m_eventMonitorThread = null;
        }

        public void readOffset(int offsetAddress, int numBytes, string destinationVariable)
        {
        }

        public void writeOffset(int offsetAddress, int numBytes, string sourceVariable)
        {

        }

        public void initialise(dynamic vaProxy)
        {
            m_vaProxy = vaProxy;
            m_connectionThread = new Thread(() => connectionRunner());
            m_connectionThread.Start();
        }

        public void shutdown()
        {
            if (m_connectionThread.IsAlive)
            {
                m_connectionThread.Abort();
            }
            if (m_eventMonitorThread.IsAlive)
            {
                m_eventMonitorThread.Abort();
            }
            if (m_isConnected)
            {
                FSUIPCConnection.Close();
            }
        }

        public FSUIPCInterface()
        {
            m_isConnected = false;
            m_vaProxy = null;
        }
    }
}
