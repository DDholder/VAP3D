using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using System.Collections.Generic;
using Moq;

namespace VAP3DUnitTests
{
    [TestClass]
    public class VoiceAttackPluginTest
    {
        [TestMethod]
        public void VoiceAttackPluginTests_InitialisesNewFSUIPCInterface()
        {
            var mockInterface = new Mock<IFSUIPCInterface>();
            var mockFactory = new Mock<IFSUIPCFactory>();
            mockFactory.Setup(factory => factory.createFSUIPCInterface(It.IsAny<IFSUIPC>(), 
                It.IsAny<IOffsetFactory>())).Returns(mockInterface.Object);

            MyVAProxy proxy = new MyVAProxy();

            VoiceAttackPlugin.SetFSUIPCFactory(proxy, mockFactory.Object);

            // Call Init1
            VoiceAttackPlugin.VA_Init1(proxy);

            Assert.IsTrue(proxy.SessionState.ContainsKey(
                VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE));
            Assert.IsInstanceOfType(proxy.SessionState[
                VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE], typeof(IFSUIPCInterface));

            mockInterface.Verify(x => x.initialise(proxy), Times.AtMost(1));
        }

        [TestMethod]
        public void VoiceAttackPluginTests_ShutdownFSUIPCInterfaceOnExit()
        {
            var mockInterface = new Mock<IFSUIPCInterface>();
            MyVAProxy proxy = new MyVAProxy();

            proxy.SessionState.Add(VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE,
                mockInterface.Object);

            // Call Exit1
            VoiceAttackPlugin.VA_Exit1(proxy);

            mockInterface.Verify(x => x.shutdown(), Times.AtMost(1));
        }

        [TestMethod]
        public void VoiceAttackPluginTests_InvokesMethodAndArgsOnFSUIPCInterface_1()
        {
            var mockInterface = new Mock<IFSUIPCInterface>();
            MyVAProxy proxy = new MyVAProxy();

            proxy.SessionState.Add(VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE,
                mockInterface.Object);
            proxy.Context = "readOffset:ABC;2;myVar";

            // Call Invoke1
            VoiceAttackPlugin.VA_Invoke1(proxy);

            mockInterface.Verify(x => x.readOffset(0xABC, typeof(short), "myVar"), Times.AtMost(1));
        }

        [TestMethod]
        public void VoiceAttackPluginTests_InvokesMethodAndArgsOnFSUIPCInterface_2()
        {
            var mockInterface = new Mock<IFSUIPCInterface>();
            MyVAProxy proxy = new MyVAProxy();

            proxy.SessionState.Add(VoiceAttackPlugin.SESSIONSTATE.KEY_FSUIPCINTERFACE,
                mockInterface.Object);
            proxy.Context = "beginMonitoringEvents:";

            // Call Invoke1
            VoiceAttackPlugin.VA_Invoke1(proxy);

            mockInterface.Verify(x => x.beginMonitoringEvents(), Times.AtMost(1));
        }
    }
}
