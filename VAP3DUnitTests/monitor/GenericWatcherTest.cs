using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using Moq;

namespace VAP3DUnitTests.monitor
{
    [TestClass]
    public class GenericWatcherTest
    {
        private class MyMonitor : GenericMonitor
        {
            public override Type getOffsetDataType()
            {
                return typeof(int);
            }

            public override void valueChanged(object value, dynamic vaProxy)
            {
                //processGenericWatch(value, vaProxy);
            }

            protected override Type getDataType()
            {
                return typeof(int);
            }
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_Equals()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(50, Watcher.WatchCondition.EqualTo, "MyIdent");

            mon.valueChanged(49, mockProxy.Object);
            mon.valueChanged(50, mockProxy.Object);
            mon.valueChanged(51, mockProxy.Object);
            mon.valueChanged(50, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_Not()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(50, Watcher.WatchCondition.Not, "MyIdent");

            mon.valueChanged(50, mockProxy.Object);
            mon.valueChanged(49, mockProxy.Object);
            mon.valueChanged(51, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_GT()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(24.56, Watcher.WatchCondition.GreaterThan, "MyIdent");

            mon.valueChanged(24.34, mockProxy.Object);
            mon.valueChanged(24.5, mockProxy.Object);
            mon.valueChanged(24.56, mockProxy.Object);
            mon.valueChanged(24.57, mockProxy.Object);
            mon.valueChanged(24.58, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_GE()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(24.56, Watcher.WatchCondition.GreaterThanEqualTo, "MyIdent");

            mon.valueChanged(24.34, mockProxy.Object);
            mon.valueChanged(24.5, mockProxy.Object);
            mon.valueChanged(24.56, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_LT()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(50, Watcher.WatchCondition.LessThan, "MyIdent");

            mon.valueChanged(51, mockProxy.Object);
            mon.valueChanged(50, mockProxy.Object);
            mon.valueChanged(49, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }

        [TestMethod]
        public void GenericWatcherTest_FiresEventWhenConditionMet_LE()
        {
            var mockProxy = new Mock<MyVAProxy>();
            mockProxy.Setup(x => x.CommandExists(It.IsAny<string>())).Returns(true);

            MyMonitor mon = new MyMonitor();
            mon.addGenericWatcher(50, Watcher.WatchCondition.LessThanEqualTo, "MyIdent");

            mon.valueChanged(55, mockProxy.Object);
            mon.valueChanged(50, mockProxy.Object);

            mockProxy.Verify(x => x.ExecuteCommand(It.Is<string>(s => s.Equals("_VAP3D_Watcher_MyIdent"))), Times.Once);
        }
    }
}
