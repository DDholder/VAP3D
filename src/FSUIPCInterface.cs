using System;
using System.Threading;
using System.Collections.Generic;
using FSUIPC;

namespace VAP3D
{
    public class FSUIPCInterface : IFSUIPCInterface
    {
        private static int CONNECT_SLEEP_TIME = 1000;
        private static int EVENT_LOOP_DELAY = 250;

        private dynamic m_vaProxy;
        private bool m_isConnected;

        private object m_eventLock = new object();

        private Thread m_connectionThread;
        private Thread m_eventMonitorThread;
        private CancellationTokenSource m_cts = new CancellationTokenSource();

        private Dictionary<int, Tuple<Offset, IMonitor>> m_monitoredOffsets = new Dictionary<int, Tuple<Offset, IMonitor>>();

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

        private Dictionary<string, object> getMonitorContext()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            // Get the altimeter setting (feet/meters)
            Offset<short> altimeterSetting = new Offset<short>(0x0C18);

            FSUIPCConnection.Process();

            context.Add("altimeterSetting", altimeterSetting.Value);

            return context;
        }

        private void setupMonitorOffsets(Dictionary<string, object> context)
        {
            // On ground flag
            Tuple<Offset, IMonitor> groundFlagMonitorTuple = new Tuple<Offset, IMonitor>(
                new Offset(0x0366, 2), new GroundFlagMonitor()
            );
            m_monitoredOffsets.Add(0x0366, groundFlagMonitorTuple);

            // Groundspeed
            Tuple<Offset, IMonitor> groundSpeedMonitorTuple = new Tuple<Offset, IMonitor>(
                new Offset(0x02B4, 4), new GroundSpeedMonitor()
                );
            m_monitoredOffsets.Add(0x02B4, groundSpeedMonitorTuple);

            // VS
            Tuple<Offset, IMonitor> verticalSpeedMonitorTuple = new Tuple<Offset, IMonitor>(
                new Offset(0x02C8, 4), new VerticalSpeedMonitor()
                );
            m_monitoredOffsets.Add(0x02C8, verticalSpeedMonitorTuple);

            // Altitude
            Tuple<Offset, IMonitor> altitudeMonitorTuple = new Tuple<Offset, IMonitor>(
                new Offset(0x3324, 4), new AltimeterMonitor(Convert.ToInt16(context["altimeterSetting"]))
                );
            m_monitoredOffsets.Add(0x3324, altitudeMonitorTuple);
        }

        private void eventMonitorJob(CancellationToken token)
        {
            // Get some context
            Dictionary<string, object> monitorContext = getMonitorContext(); 

            setupMonitorOffsets(monitorContext);

            while (!token.IsCancellationRequested)
            {
                FSUIPCConnection.Process();

                foreach (KeyValuePair<int, Tuple<Offset, IMonitor>> kv in m_monitoredOffsets)
                {
                    IMonitor monitor = kv.Value.Item2;
                    Type monitorType = monitor.getOffsetDataType();

                    monitor.valueChanged(kv.Value.Item1.GetValue(monitorType), m_vaProxy);
                }

                // Process
                Thread.Sleep(EVENT_LOOP_DELAY);
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

            m_eventMonitorThread = new Thread(() => eventMonitorJob(m_cts.Token));
            m_eventMonitorThread.Start();
        }

        public void stopMonitoringEvents()
        {
            if (!m_isConnected)
            {
                writeErrorToLog("Not connected");
                return;
            }

            m_cts.Cancel();
            m_cts.Dispose();

            m_eventMonitorThread = null;
        }

        public void addMetricToMonitor(int offset, object value, int conditionFlag, string identifier)
        {
            Tuple<Offset, IMonitor> eventTuple = m_monitoredOffsets[offset];
            eventTuple.Item2.addGenericWatcher(value, (Watcher.WatchCondition)conditionFlag, identifier);
        }

        public void readOffset(int offset, int numBytes, string destinationVariable)
        {  
        }

        public void writeOffset(int offset, int numBytes, string sourceVariable)
        {
        }

        public void initialise(dynamic vaProxy)
        {
            m_vaProxy = vaProxy;
            m_connectionThread = new Thread(() => connectionJob(m_cts.Token));
            m_connectionThread.Start();
        }

        public void shutdown()
        {
            m_cts.Cancel();
            m_cts.Dispose();

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
