using System;
using System.Threading;
using System.Collections.Generic;
using FSUIPC;

namespace VAP3D
{
    public class FSUIPCInterface : IFSUIPCInterface
    {
        private static int CONNECT_SLEEP_TIME = 2000;

        private dynamic m_vaProxy = null;
        private bool m_isConnected = false;

        private EventMonitor m_eventMonitor = null;
        private Thread m_connectionThread = null;
        private CancellationTokenSource m_cts = new CancellationTokenSource();

        // -----------------------------------------
        // Private methods
        private void connectionJob(CancellationToken token)
        {
            while (!m_isConnected && !token.IsCancellationRequested)
            {
                try
                {
                    FSUIPCConnection.Open();

                    m_isConnected = true;
                    m_vaProxy.WriteToLog("Connected to FSUIPC", "green");
                }
                catch (Exception e)
                {
                    Thread.Sleep(CONNECT_SLEEP_TIME);
                }
            }
        }

        private void writeErrorToLog(string error)
        {
            m_vaProxy.WriteToLog("VA:P3D Error: " + error, "red");
        }

        // -----------------------------------------
        // Public Interface
        public void beginMonitoringEvents()
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }

            if (m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor already running. Stop it first with `stopMonitoringEvents:`");
                return;
            }

            m_eventMonitor.start();
        }

        public void stopMonitoringEvents()
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }

            if (!m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor is not running.");
                return;
            }

            m_eventMonitor.stop();
        }

        public void addMetricToMonitor(int offset, object value, int conditionFlag, string identifier)
        {
            if (!m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor is not currently running. Call `beginMonitoringEvents:` first");
                return;
            }

            m_eventMonitor.addMetricToMonitor(offset, value, conditionFlag, identifier);
        }

        public void readOffset(int offset, int numBytes, string destinationVariable)
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }
        }

        public void writeOffset(int offset, int numBytes, string sourceVariable)
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }
        }

        public void initialise(dynamic vaProxy)
        {
            m_vaProxy = vaProxy;
            m_connectionThread = new Thread(() => connectionJob(m_cts.Token));
            m_connectionThread.Start();

            m_eventMonitor = new EventMonitor(vaProxy);
        }

        public void shutdown()
        {
            m_cts.Cancel();
            m_cts.Dispose();

            if (m_eventMonitor.isRunning())
            {
                m_eventMonitor.stop();
            }

            if (m_isConnected)
            {
                FSUIPCConnection.Close();
            }
        }
    }
}
