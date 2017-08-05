using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VAP3D
{
    public class EventMonitor
    {
        private static int EVENT_LOOP_DELAY = 250;

        private object m_lock = new Object();

        private Thread m_thread = null;
        private CancellationTokenSource m_cts = null;
        private dynamic m_vaProxy = null;

        private Dictionary<int, Tuple<Offset, IMonitor>> m_monitoredOffsets = new Dictionary<int, Tuple<Offset, IMonitor>>();

        private Dictionary<string, object> getMonitorContext()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            // Get the altimeter setting (feet/meters)
            Offset<short> altimeterSetting = new Offset<short>(0x0C18);

            FSUIPCConnection.Process();

            context.Add("altimeterSetting", altimeterSetting.Value);

            return context;
        }

        private void startMonitoringOffset(int offset, int byteSize, IMonitor monitor)
        {
            Tuple<Offset, IMonitor> eventTuple = new Tuple<Offset, IMonitor>(
                new Offset(offset, byteSize), monitor);

            m_monitoredOffsets.Add(offset, eventTuple);
        }

        private void setupDefaultMonitorOffsets(Dictionary<string, object> context)
        {
            startMonitoringOffset(0x0366, 2, new GroundFlagMonitor()); // On ground flag
            startMonitoringOffset(0x02B4, 4, new GroundSpeedMonitor()); // Groundspeed
            startMonitoringOffset(0x02C8, 4, new VerticalSpeedMonitor()); // VS
            startMonitoringOffset(0x3324, 4, new AltimeterMonitor(Convert.ToInt16(context["altimeterSetting"]))); // Altitude
        }

        private void run(CancellationToken token, dynamic vaProxy)
        {
            // Get some context
            Dictionary<string, object> monitorContext = getMonitorContext();

            setupDefaultMonitorOffsets(monitorContext);

            while (!token.IsCancellationRequested)
            {
                FSUIPCConnection.Process();

                lock (m_lock)
                {
                    foreach (KeyValuePair<int, Tuple<Offset, IMonitor>> kv in m_monitoredOffsets)
                    {
                        IMonitor monitor = kv.Value.Item2;
                        Type monitorType = monitor.getOffsetDataType();

                        monitor.valueChanged(kv.Value.Item1.GetValue(monitorType), vaProxy);
                    }
                }

                // Process
                Thread.Sleep(EVENT_LOOP_DELAY);
            }

            // Tidy up
            m_monitoredOffsets.Clear();
        }

        public bool addMetricToMonitor(int offset, object value, int conditionFlag, string identifier)
        {
            lock(m_lock)
            {
                if (!m_monitoredOffsets.ContainsKey(offset))
                {
                    return false;
                }

                Tuple<Offset, IMonitor> eventTuple = m_monitoredOffsets[offset];
                eventTuple.Item2.addGenericWatcher(value, (Watcher.WatchCondition)conditionFlag, identifier);
            }

            return true;
        }

        public void start()
        {
            m_cts = new CancellationTokenSource();

            m_thread = new Thread(() => run(m_cts.Token, m_vaProxy));
            m_thread.Start();
        }

        public void stop()
        {
            m_cts.Cancel();
            m_cts.Dispose();

            m_thread.Join();
        }

        public bool isRunning()
        {
            return m_thread.IsAlive;
        }

        public EventMonitor(dynamic vaProxy)
        {
            m_vaProxy = vaProxy;
        }
    }
}
