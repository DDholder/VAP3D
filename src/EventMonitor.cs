using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FSUIPC;

namespace VAP3D
{
    public class EventMonitor
    {
        private static int EVENT_LOOP_DELAY = 250;

        private object m_lock = new Object();

        private Thread m_thread = null;
        private CancellationTokenSource m_cts = null;
        private dynamic m_vaProxy = null;
        private IFSUIPC m_fsuipc = null;
        private IOffsetFactory m_offsetFactory = null;

        private Dictionary<int, Tuple<IOffset, IMonitor>> m_monitoredOffsets = new Dictionary<int, Tuple<IOffset, IMonitor>>();

        private Dictionary<string, object> getMonitorContext()
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            // Get the altimeter setting (feet/meters)
            IOffset<short> altimeterSetting = m_offsetFactory.createOffset<short>(OffsetValues.AltimeterSetting);

            m_fsuipc.Process();

            context.Add("altimeterSetting", altimeterSetting.Value);

            return context;
        }

        private void startMonitoringOffset(int offset, int byteSize, IMonitor monitor)
        {
            Tuple<IOffset, IMonitor> eventTuple = new Tuple<IOffset, IMonitor>(
                m_offsetFactory.createOffset(offset, byteSize), monitor);

            m_monitoredOffsets.Add(offset, eventTuple);
        }

        private void setupDefaultMonitorOffsets(Dictionary<string, object> context)
        {
            startMonitoringOffset(OffsetValues.GroundFlag, 2, new GroundFlagMonitor()); // On ground flag
            startMonitoringOffset(OffsetValues.GroundSpeed, 4, new GroundSpeedMonitor()); // Groundspeed
            startMonitoringOffset(OffsetValues.VerticalSpeed_InAir, 4, new VerticalSpeedMonitor()); // VS
            startMonitoringOffset(OffsetValues.Altimeter, 4, new AltimeterMonitor(Convert.ToInt16(context["altimeterSetting"]))); // Altitude
        }

        private void run(CancellationToken token, dynamic vaProxy)
        {
            // Get some context
            Dictionary<string, object> monitorContext = getMonitorContext();

            setupDefaultMonitorOffsets(monitorContext);

            while (!token.IsCancellationRequested)
            {
                m_fsuipc.Process();

                lock (m_lock)
                {
                    foreach (KeyValuePair<int, Tuple<IOffset, IMonitor>> kv in m_monitoredOffsets)
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

                Tuple<IOffset, IMonitor> eventTuple = m_monitoredOffsets[offset];
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
            if (m_thread == null)
                return false;

            return m_thread.IsAlive;
        }

        public EventMonitor(dynamic vaProxy, IFSUIPC fsuipc, IOffsetFactory offsetFactory)
        {
            m_vaProxy = vaProxy;
            m_fsuipc = fsuipc;
            m_offsetFactory = offsetFactory;
        }
    }
}
