using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using System.Collections.Generic;

namespace VAP3DUnitTests
{
    [TestClass]
    public class VoiceAttackPluginTest
    {
        public class MockInterface : IFSUIPCInterface
        {
            public int Called = 0x0;

            public void beginMonitoringEvents()
            {
                Called |= 0x1;
            }

            public void initialise(dynamic vaProxy)
            {
                Called |= 0x2;
            }

            public void readOffset(int offsetAddress, int numBytes, string destinationVariable)
            {
                Called |= 0x4;
            }

            public void shutdown()
            {
                Called |= 0x8;
            }

            public void stopMonitoringEvents()
            {
                Called |= 0x10;
            }

            public void writeOffset(int offsetAddress, int numBytes, string sourceVariable)
            {
                Called |= 0x20;
            }
        }

        public class MockFactory : IFSUIPCFactory
        {
            IFSUIPCInterface mInterface = null;

            public void setInterface(IFSUIPCInterface fsuipcInterface)
            {
                mInterface = fsuipcInterface;
            }

            public IFSUIPCInterface createFSUIPCInterface()
            {
                return mInterface;
            }
        }

        public class MyVAProxy
        {
            public Dictionary<String, object> SessionState;
            public string Context;

            public MyVAProxy()
            {
                SessionState = new Dictionary<string, object>();
            }
        }

        [TestMethod]
        public void InitialisesNewFSUIPCInterface()
        {
            MockInterface mockInterface = new MockInterface();
            MyVAProxy proxy = new MyVAProxy();
            MockFactory factory = new MockFactory();
            factory.setInterface(mockInterface);

            VoiceAttackPlugin.SetFSUIPCFactory(proxy, factory);

            // Call Init1
            VoiceAttackPlugin.VA_Init1(proxy);

            Assert.IsTrue(proxy.SessionState.ContainsKey(
                VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE));
            Assert.IsInstanceOfType(proxy.SessionState[
                VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE], typeof(MockInterface));

            Assert.AreEqual(mockInterface.Called, 0x2);
        }

        [TestMethod]
        public void ShutdownFSUIPCInterfaceOnExit()
        {
            MockInterface mockInterface = new MockInterface();
            MyVAProxy proxy = new MyVAProxy();

            proxy.SessionState.Add(VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE,
                mockInterface);

            // Call Exit1
            VoiceAttackPlugin.VA_Exit1(proxy);

            Assert.AreEqual(mockInterface.Called, 0x8);
        }

        [TestMethod]
        public void InvokesMethodAndArgsOnFSUIPCInterface_1()
        {
            MockInterface mockInterface = new MockInterface();
            MyVAProxy proxy = new MyVAProxy();

            proxy.SessionState.Add(VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE,
                mockInterface);
            proxy.Context = "readOffset:ABC;2;myVar";

            // Call Invoke1
            VoiceAttackPlugin.VA_Invoke1(proxy);

            Assert.AreEqual(mockInterface.Called, 0x4);
        }

        [TestMethod]
        public void InvokesMethodAndArgsOnFSUIPCInterface_2()
        {
        }

        [TestMethod]
        public void InvokesMethodAndArgsOnFSUIPCInterface_3()
        {
        }
    }
}
