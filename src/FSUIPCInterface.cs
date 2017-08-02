using System;
using System.Threading;
using System.Collections.Generic;
using FSUIPC;

namespace VAP3D
{
    public class FSUIPCInterface : IFSUIPCInterface
    {
        private interface IMonitorEvent
        {
            void valueChanged(object value, dynamic vaProxy);
            void addGenericWatch(object value, WatchConditions condition, string identifier);
        }

        private struct GenericWatch
        {
            public object referenceValue;
            public WatchConditions condition;
            public string label;
            public bool processed;
        }

        private abstract class GenericMonitor : IMonitorEvent
        {
            protected List<GenericWatch> mGenericWatches = new List<GenericWatch>();

            protected bool doCompare(WatchConditions condition, dynamic a, dynamic b)
            {
                switch(condition)
                {
                    case WatchConditions.EqualTo:
                        return a == b;
                    case WatchConditions.GreaterThan:
                        return a > b;
                    case WatchConditions.GreaterThanEqualTo:
                        return a >= b;
                    case WatchConditions.LessThan:
                        return a < b;
                    case WatchConditions.LessThanEqualTo:
                        return a <= b;
                    case WatchConditions.Not:
                        return a != b;
                }

                return false;
            }

            protected void processGenericWatch(object value, dynamic vaProxy)
            {
                for (int i = 0; i < mGenericWatches.Count; ++i)
                {
                    GenericWatch watch = mGenericWatches[i];
                    if (!watch.processed)
                    {
                        Type t = getDataType();
                        dynamic ourValue = Convert.ChangeType(value, t);
                        dynamic refVal = Convert.ChangeType(watch.referenceValue, t);

                        if (doCompare(watch.condition, ourValue, refVal))
                        {
                            vaProxy.ExecuteFunction("_VAP3D_Generic_" + watch.label);
                            watch.processed = true;
                        }
                    }
                }
            }

            public void addGenericWatch(object value, WatchConditions condition, string identifier)
            {
                GenericWatch watch = new GenericWatch();
                watch.referenceValue = value;
                watch.condition = condition;
                watch.label = identifier;
                watch.processed = false;

                mGenericWatches.Add(watch);
            }

            public virtual void valueChanged(object value, dynamic vaProxy)
            {
                throw new NotImplementedException();
            }

            protected abstract Type getDataType();
        }

        private class GroundSpeedMonitor : GenericMonitor
        {
            private bool mCalledOut80Kts = false;

            public override void valueChanged(object value, dynamic vaProxy)
            {
                int speedFixedPoint = (int)value;

                double speed = ((double)speedFixedPoint / 65535.0);
                if (speed > 80.0 && !mCalledOut80Kts)
                {
                    vaProxy.ExecuteFunction("_VAP3D_EightyKnots");
                    mCalledOut80Kts = true;
                }

                processGenericWatch(speed, vaProxy);
            }

            protected override Type getDataType()
            {
                return typeof(double);
            }
        }

        private static int CONNECT_SLEEP_TIME = 1000;
        private static int EVENT_LOOP_DELAY = 250;

        private dynamic m_vaProxy;

        private bool m_isConnected;

        private object m_eventLock = new object();
        private Thread m_connectionThread;
        private Thread m_eventMonitorThread;

        private CancellationTokenSource m_cts = new CancellationTokenSource();

        private List<Tuple<Offset, IMonitorEvent>> m_monitoredOffsets = new List<Tuple<Offset, IMonitorEvent>>();

        private enum MonitoredOffsets
        {
            OnGroundFlag,
            Groundspeed,
            VerticalSpeed,
            Altitude,

            NUM_OFFSETS
        }

        private enum WatchConditions
        {
            LessThan,
            LessThanEqualTo,
            EqualTo,
            GreaterThanEqualTo,
            GreaterThan,
            Not
        }

        // -----------------------------------------
        // Private methods
        private void connectionJob(CancellationToken token)
        {
            while (!m_isConnected && token.IsCancellationRequested)
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

        private void eventMonitorJob(CancellationToken token)
        {
            setupMonitorOffsets();

            while (token.IsCancellationRequested)
            {
                FSUIPCConnection.Process();

                for (int i = 0; i < m_monitoredOffsets.Count; ++i)
                {
                    Tuple<Offset, IMonitorEvent> eventTuple = m_monitoredOffsets[i];
                    eventTuple.Item2.valueChanged(eventTuple.Item1.Value, m_vaProxy);
                }

                // Process
                Thread.Sleep(EVENT_LOOP_DELAY);
            }
        }

        private void writeErrorToLog(string error)
        {
            m_vaProxy.WriteToLog("VA:P3D Error: " + error, "red");
        }

        private void setupMonitorOffsets()
        {
            // On ground flag
            // Groundspeed
            // VS
            // Altitude
            Offset groundspeedOffset = new Offset(0xABCD);
            Tuple<Offset, IMonitorEvent> eventTuple = new Tuple<Offset, IMonitorEvent>(
                groundspeedOffset, new GroundSpeedMonitor());

            m_monitoredOffsets.Add(eventTuple);
        }

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

        public void addMetricToMonitor(int metricId, object value, int conditionFlag, string identifier)
        {
            Tuple<Offset, IMonitorEvent> eventTuple = m_monitoredOffsets[metricId];
            eventTuple.Item2.addGenericWatch(value, (WatchConditions)conditionFlag, identifier);
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
