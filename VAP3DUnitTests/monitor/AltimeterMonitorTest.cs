using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using Moq;

namespace VAP3DUnitTests.monitor
{
    [TestClass]
    public class AltimeterMonitorTest
    {
        private const int SettingFeet = 1;
        private const int SettingMeters = 2;

        [TestMethod]
        public void AltimeterMonitorTest_CallsOutWhenClimbing()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            AltimeterMonitor mon = new AltimeterMonitor(SettingFeet);

            mon.valueChanged(9000, mockProxy.Object);
            mon.valueChanged(9500, mockProxy.Object);
            mon.valueChanged(9999, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(12000, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_TenThousandFeet"))), Times.Once);
        }

        [TestMethod]
        public void AltimeterMonitorTest_CallsOutWhenDescending()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            AltimeterMonitor mon = new AltimeterMonitor(SettingFeet);

            mon.valueChanged(11000, mockProxy.Object);
            mon.valueChanged(10500, mockProxy.Object);
            mon.valueChanged(10001, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(9998, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_TenThousandFeet"))), Times.Once);
        }

        [TestMethod]
        public void AltimeterMonitorTest_CallsOutAtCorrectAltitudeWhenSettingIsMeters()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            AltimeterMonitor mon = new AltimeterMonitor(SettingMeters);

            mon.valueChanged((int)(10010 * 0.3048), mockProxy.Object);
            mon.valueChanged((int)(10000 * 0.3048), mockProxy.Object);
            mon.valueChanged((int)(9998  * 0.3048), mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_TenThousandFeet"))), Times.Once);
        }

        [TestMethod]
        public void AltimeterMonitorTest_DoesNotSpamCalloutsWhenCruising()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            AltimeterMonitor mon = new AltimeterMonitor(SettingFeet);

            mon.valueChanged(9500, mockProxy.Object);
            mon.valueChanged(9999, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(11000, mockProxy.Object);
            mon.valueChanged(10500, mockProxy.Object);
            mon.valueChanged(10001, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(9998, mockProxy.Object);
            mon.valueChanged(10001, mockProxy.Object);
            mon.valueChanged(9995, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_TenThousandFeet"))), Times.Once);
        }

        [TestMethod]
        public void AltimeterMonitorTest_CallsOutWhenDescendingAfterClimbing()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            AltimeterMonitor mon = new AltimeterMonitor(SettingFeet);

            mon.valueChanged(9000, mockProxy.Object);
            mon.valueChanged(9500, mockProxy.Object);
            mon.valueChanged(9999, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(12000, mockProxy.Object);
            mon.valueChanged(11000, mockProxy.Object);

            System.Threading.Thread.Sleep((int)AltimeterMonitor.CallThresholdMs + 1000);

            mon.valueChanged(10500, mockProxy.Object);
            mon.valueChanged(10001, mockProxy.Object);
            mon.valueChanged(10000, mockProxy.Object);
            mon.valueChanged(9998, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_TenThousandFeet"))), Times.Exactly(2));
        }
    }
}
