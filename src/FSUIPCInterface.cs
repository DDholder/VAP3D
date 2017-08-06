using System;
using System.Threading;
using System.Collections.Generic;
using FSUIPC;

namespace VAP3D
{
    public class FSUIPCInterface : IFSUIPCInterface
    {
        private static int CONNECT_SLEEP_TIME = 2000;

        private IFSUIPC m_fsuipc;
        private IOffsetFactory m_offsetFactory;

        private dynamic m_vaProxy = null;
        private bool m_isConnected = false;
        private bool m_connectionThreadLaunched = false;
        private Object m_connectionLock = new Object();

        private EventMonitor m_eventMonitor = null;
        private Thread m_connectionThread = null;
        private CancellationTokenSource m_cts = new CancellationTokenSource();

        /// <summary>
        /// Starts a thread which repeatedly attempts to make a connection to
        /// FSUIPC.
        /// </summary>
        /// <param name="token">a cancellation token</param>
        /// <param name="sync">a condition object to be notified when the thread starts</param>
        private void connectionJob(CancellationToken token, Object sync)
        {
            while (!m_isConnected && !token.IsCancellationRequested)
            {
                try
                {
                    lock (sync)
                    {
                        m_connectionThreadLaunched = true;
                        Monitor.Pulse(sync);
                    }

                    lock (m_connectionLock)
                    {
                        m_fsuipc.Open();
                        m_isConnected = true;
                    }

                    m_vaProxy.WriteToLog("Connected to FSUIPC", "green");
                }
                catch (Exception e)
                {
                    Thread.Sleep(CONNECT_SLEEP_TIME);
                }
            }
        }

        /// <summary>
        /// Writes to the VoiceAttack log
        /// </summary>
        /// <param name="error"></param>
        private void writeErrorToLog(string error)
        {
            m_vaProxy.WriteToLog("VA:P3D Error: " + error, "red");
        }

        /// <summary>
        /// Starts monitoring a default set of data and sends events to VoiceAttack
        /// based on a number of conditions. Read the docs for more information.
        /// </summary>
        public void beginMonitoringEvents()
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            if (m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor already running. Stop it first with `stopMonitoringEvents:`");
                return;
            }

            m_eventMonitor.start();
        }

        /// <summary>
        /// Stops monitoring default data.
        /// </summary>
        public void stopMonitoringEvents()
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            if (!m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor is not running.");
                return;
            }

            m_eventMonitor.stop();
        }

        /// <summary>
        /// Adds a metric and condition to be 'evented' upon by monitoring a default set of
        /// data. If the condition is met, the event is fired and a callback is sent to
        /// VoiceAttack. Read the docs for more information.
        /// </summary>
        /// <param name="offset">the offset address to monitor</param>
        /// <param name="value">the reference value for the condition</param>
        /// <param name="conditionFlag">the condition which will evaluate the refernece against the monitored value</param>
        /// <param name="identifier">an identifer which will form a VoiceAttack command which will be executed should the condition be met</param>
        public void addMetricToMonitor(int offset, object value, int conditionFlag, string identifier)
        {
            if (!m_eventMonitor.isRunning())
            {
                writeErrorToLog("Event monitor is not currently running. Call `beginMonitoringEvents:` first");
                return;
            }

            m_eventMonitor.addMetricToMonitor(offset, value, conditionFlag, identifier);
        }

        /// <summary>
        /// Reads an FSUIPC offset and updates a VoiceAttack variable
        /// </summary>
        /// <param name="offsetAddress">address of offset</param>
        /// <param name="dataType">the data type to read</param>
        /// <param name="sourceVariable">VoiceAttack variable which will be written to</param>
        public void readOffset(int offsetAddress, Type dataType, string destinationVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            int numBytes = Utilities.numBytesFromType(dataType);
            if (numBytes < 1)
            {
                writeErrorToLog(dataType.Name + " is an unsupported type");
                return;
            }

            IOffset offset = m_offsetFactory.createOffset(offsetAddress, numBytes);
            m_fsuipc.Process();

            if (!Utilities.setVariableValueFromOffset(dataType, offset, destinationVariable, m_vaProxy))
            {
                writeErrorToLog("Failed to set '" + destinationVariable + "' with type " + dataType.Name);
            }
        }

        /// <summary>
        /// Writes an FSUIPC offset using data supplied in a VoiceAttack variable
        /// </summary>
        /// <param name="offsetAddress">address of offset</param>
        /// <param name="dataType">the data type to write</param>
        /// <param name="sourceVariable">VoiceAttack variable containing the data</param>
        public void writeOffset(int offsetAddress, Type dataType, string sourceVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            int numBytes = Utilities.numBytesFromType(dataType);
            if (numBytes < 1)
            {
                writeErrorToLog(dataType.Name + " is an unsupported type");
                return;
            }

            IOffset offset = m_offsetFactory.createOffset(offsetAddress, numBytes, true);

            object value = Utilities.getVariableValueForOffset(dataType, sourceVariable, m_vaProxy);
            if (value != null)
            {
                offset.SetValue(value);

                m_fsuipc.Process();
            }
            else
            {
                writeErrorToLog("Variable '" + sourceVariable + "' does not contain a value for type " + dataType.Name);
            }
        }

        /// <summary>
        /// Reads an offset which contains a string and updates a Text destination variable
        /// </summary>
        /// <param name="offsetAddress">address of offset</param>
        /// <param name="length">length the length of the string data</param>
        /// <param name="destinationVariable">the Text variable destination in VoiceAttack</param>
        public void readStringOffset(int offsetAddress, int length, string destinationVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            IOffset offset = m_offsetFactory.createOffset(offsetAddress, length);
            m_fsuipc.Process();

            m_vaProxy.SetText(destinationVariable, offset.GetValue(typeof(string)));
        }

        /// <summary>
        /// Writes an offset which contains a string from a Text source variable
        /// </summary>
        /// <param name="offsetAddress">address of offset</param>
        /// <param name="length">the length of the string data</param>
        /// <param name="sourceVariable">the Text variable source in VoiceAttack</param>
        public void writeStringOffset(int offsetAddress, int length, string sourceVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            string value = m_vaProxy.GetText(sourceVariable);
            
            if (value != null)
            {
                IOffset offset = m_offsetFactory.createOffset(offsetAddress, value.Length, true);
                offset.SetValue(value);

                m_fsuipc.Process();
            }
            else
            {
                writeErrorToLog("Variable '" + sourceVariable + "' does not contain a value for Text type");
            }
        }

        /// <summary>
        /// Reads an Lvar using Majestic Software's methodoly. Specifically for the Dash 8 Q400.
        /// </summary>
        /// <param name="idcode">the Lvar idcode retrieved using the varlist spreadsheet and IntVarCRCGen.exe</param>
        /// <param name="dataType">the underlying data type to read from</param>
        /// <param name="destinationVariable">the VoiceAttack destination variable to update</param>
        public void readMjc(int idcode, Type dataType, string destinationVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            Mjc reader = new Mjc(m_fsuipc, m_offsetFactory, m_vaProxy);
            reader.read(idcode, dataType, destinationVariable, writeErrorToLog);
        }

        /// <summary>
        /// Writes an Lvar using Majestic Software's methodoly. Specifically for the Dash 8 Q400. 
        /// </summary>
        /// <param name="idcode">the Lvar idcode retrieved using the varlist spreadsheet and IntVarCRCGen.exe</param>
        /// <param name="dataType">the underlying data type to write to</param>
        /// <param name="sourceVariable">the VoiceAttack source variable to read from</param>
        public void writeMjc(int idcode, Type dataType, string sourceVariable)
        {
            lock (m_connectionLock)
            {
                if (!m_isConnected)
                {
                    writeErrorToLog("Not connected");
                    return;
                }
            }

            Mjc writer = new Mjc(m_fsuipc, m_offsetFactory, m_vaProxy);
            writer.write(idcode, dataType, sourceVariable, writeErrorToLog);
        }

        /// <summary>
        /// Initialises the interface and attempts to connect to FSUIPC.
        /// Will continue to attempt to connect until shutdown() is called,
        /// or a connection is established.
        /// </summary>
        /// <param name="vaProxy"></param>
        public void initialise(dynamic vaProxy)
        {
            Object sync = new Object();

            m_vaProxy = vaProxy;
            m_connectionThread = new Thread(() => connectionJob(m_cts.Token, sync));
            m_connectionThread.Start();

            lock(sync)
            {
                while(!m_connectionThreadLaunched)
                    Monitor.Wait(sync);
            }
            
            m_eventMonitor = new EventMonitor(vaProxy, m_fsuipc, m_offsetFactory);
        }

        /// <summary>
        /// Shutdown and disconnect the interface
        /// </summary>
        public void shutdown()
        {
            m_cts.Cancel();
            if (m_connectionThread.IsAlive)
            {
                m_connectionThread.Join();
            }

            m_cts.Dispose();

            if (m_eventMonitor.isRunning())
            {
                m_eventMonitor.stop();
            }

            if (m_isConnected)
            {
                m_fsuipc.Close();
            }
        }

        /// <summary>
        /// Returns whether the interface is connected to FSUIPC (and the sim)
        /// </summary>
        /// <returns>true if connected</returns>
        public bool isConnected()
        {
            lock (m_connectionLock)
            {
                return m_isConnected;
            }
        }

        public void beginOffsetBatch()
        {
            throw new NotImplementedException();
        }

        public void endOffsetBatch()
        {
            throw new NotImplementedException();
        }

        public void readLvar(string lvarName, Type lvarType, string destinationVariable)
        {
            throw new NotImplementedException();
        }

        public void writeLvar(string lvarName, Type lvarType, string sourceVariable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fsuipc"></param>
        /// <param name="offsetFactory"></param>
        public FSUIPCInterface(IFSUIPC fsuipc, IOffsetFactory offsetFactory)
        {
            m_isConnected = false;
            m_vaProxy = null;
            m_fsuipc = fsuipc;
            m_offsetFactory = offsetFactory;
        }
    }
}
