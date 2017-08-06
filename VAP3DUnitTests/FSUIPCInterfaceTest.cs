using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using Moq;
using System.Collections.Generic;

namespace VAP3DUnitTests
{
    [TestClass]
    public class FSUIPCInterfaceTest
    {
        [TestMethod]
        public void FSUIPCInterfaceTests_Constructs()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_InitialisesAndConnects()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            while (!fsuipcInterface.isConnected()) ;

            mockFsuipc.Verify(x => x.Open(), Times.Exactly(1));

            mockProxy.Verify(x => x.WriteToLog(
                It.IsAny<String>(),
                It.Is<String>(s => s.Equals("green"))),
                Times.Exactly(1));
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_InitialisesAndConnectsAfterTryingMultipleTimes()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            int attempts = 0;
            mockFsuipc.Setup(f => f.Open()).Callback(() =>
            {
                if (attempts == 0)
                {
                    attempts++;
                    throw new Exception();
                }
            });

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            while (!fsuipcInterface.isConnected())
            {
                System.Threading.Thread.Sleep(10); // Not the greatest, but don't have much choice. 
                                                   // Have to wait for the thread to exit before verifying.
            }

            mockFsuipc.Verify(x => x.Open(), Times.Exactly(2));

            mockProxy.Verify(x => x.WriteToLog(
                It.IsAny<String>(),
                It.Is<String>(s => s.Equals("green"))),
                Times.Exactly(1));
        }

        public enum VADataTypes
        {
            Int,
            Decimal,
            Bool,
            String
        }

        public void ReadOffsetTest(Type type, int numBytes, object variableValue, VADataTypes typeToSet, Type typeToRead)
        {
            const int address = 0x1234;
            const String destVar = "myVar";

            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            var mockedOffset = new Mock<IOffset>();
            mockedOffset.Setup(x => x.GetValue(It.Is<Type>(t => t.Equals(typeToRead)))).Returns(variableValue);

            mockOffsetFactory.Setup(x => x.createOffset(It.IsAny<int>(), It.IsAny<int>())).Returns(
                mockedOffset.Object);

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            fsuipcInterface.readOffset(address, type, destVar);

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))), Times.Never);
            mockFsuipc.Verify(x => x.Process(), Times.Exactly(1));
            mockOffsetFactory.Verify(x => x.createOffset(address, numBytes), Times.Exactly(1));
            mockedOffset.Verify(x => x.GetValue(It.Is<Type>(t => t.Equals(typeToRead))));
            if (typeToSet == VADataTypes.Int)
            {
                mockProxy.Verify(x => x.SetInt(destVar, Convert.ToInt64(variableValue)), Times.Once());
            }
            else if (typeToSet == VADataTypes.Decimal)
            {
                mockProxy.Verify(x => x.SetDecimal(destVar, Convert.ToDecimal(variableValue)), Times.Once());
            }
            else if (typeToSet == VADataTypes.Bool)
            {
                mockProxy.Verify(x => x.SetBoolean(destVar, Convert.ToBoolean(variableValue)), Times.Once());
            }
            else
            {
                throw new Exception("Invalid data type to test!");
            }
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_CanReadOffset()
        {
            // Int types
            ReadOffsetTest(typeof(Char), 1, 50, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(Byte), 1, Byte.MaxValue, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(Int16), 2, 5, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(UInt16), 2, UInt16.MaxValue, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(Int32), 4, Int32.MinValue, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(UInt32), 4, 128, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(Int64), 8, Int64.MaxValue, VADataTypes.Int, typeof(Int64));
            ReadOffsetTest(typeof(UInt64), 8, 50, VADataTypes.Int, typeof(Int64));

            // Decimal types
            ReadOffsetTest(typeof(Double), 8, 10.4583, VADataTypes.Decimal, typeof(Decimal));
            ReadOffsetTest(typeof(Single), 4, -1.0001, VADataTypes.Decimal, typeof(Decimal));

            // Boolean types
            ReadOffsetTest(typeof(Boolean), 1, true, VADataTypes.Bool, typeof(Boolean));
            ReadOffsetTest(typeof(Boolean), 1, false, VADataTypes.Bool, typeof(Boolean));
        }

        public void WriteOffsetTest(Type type, int numBytes, object variableValue, VADataTypes typeToGet)
        {
            const int address = 0x1234;
            const String sourceVar = "mySourceVar";

            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            var mockedOffset = new Mock<IOffset>();

            mockOffsetFactory.Setup(x => x.createOffset(It.IsAny<int>(),
                It.Is<int>(i => i.Equals(numBytes)), true)).Returns(
                    mockedOffset.Object);

            mockProxy.Setup(x => x.GetInt(It.IsAny<string>())).Returns(Convert.ToInt64(variableValue));
            mockProxy.Setup(x => x.GetBoolean(It.IsAny<string>())).Returns(Convert.ToBoolean(variableValue));
            mockProxy.Setup(x => x.GetDecimal(It.IsAny<string>())).Returns(Convert.ToDecimal(variableValue));

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            fsuipcInterface.writeOffset(address, type, sourceVar);

            mockFsuipc.Verify(x => x.Process(), Times.Exactly(1));
            mockOffsetFactory.Verify(x => x.createOffset(address, numBytes, true), Times.Exactly(1));

            if (typeToGet == VADataTypes.Int)
            {
                mockProxy.Verify(x => x.GetInt(sourceVar), Times.Once());
                mockedOffset.Verify(x => x.SetValue(Convert.ToInt64(variableValue)));
            }
            else if (typeToGet == VADataTypes.Decimal)
            {
                mockProxy.Verify(x => x.GetDecimal(sourceVar), Times.Once());
                mockedOffset.Verify(x => x.SetValue(Convert.ToDecimal(variableValue)));
            }
            else if (typeToGet == VADataTypes.Bool)
            {
                mockProxy.Verify(x => x.GetBoolean(sourceVar), Times.Once());
                mockedOffset.Verify(x => x.SetValue(Convert.ToBoolean(variableValue)));
            }
            else
            {
                throw new Exception("Invalid data type to test!");
            }
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_CanWriteOffset()
        {
            // Int types
            WriteOffsetTest(typeof(Char), 1, 50, VADataTypes.Int);
            WriteOffsetTest(typeof(Byte), 1, Byte.MaxValue, VADataTypes.Int);
            WriteOffsetTest(typeof(Int16), 2, 5, VADataTypes.Int);
            WriteOffsetTest(typeof(UInt16), 2, UInt16.MaxValue, VADataTypes.Int);
            WriteOffsetTest(typeof(Int32), 4, Int32.MinValue, VADataTypes.Int);
            WriteOffsetTest(typeof(UInt32), 4, 128, VADataTypes.Int);
            WriteOffsetTest(typeof(Int64), 8, Int64.MaxValue, VADataTypes.Int);
            WriteOffsetTest(typeof(UInt64), 8, 50, VADataTypes.Int);

            // Decimal types
            WriteOffsetTest(typeof(Double), 8, 10.4583, VADataTypes.Decimal);
            WriteOffsetTest(typeof(Single), 4, -1.0001, VADataTypes.Decimal);

            // Boolean types
            WriteOffsetTest(typeof(Boolean), 1, true, VADataTypes.Bool);
            WriteOffsetTest(typeof(Boolean), 1, false, VADataTypes.Bool);
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_FailsWriteOffsetNotConnected()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            mockFsuipc.Setup(x => x.Open()).Throws(new Exception());

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);
            fsuipcInterface.writeOffset(0x1234, typeof(Int32), "myVar");

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))));

            fsuipcInterface.shutdown();
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_FailsWriteOffsetIncorrectDataType()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);
            fsuipcInterface.writeOffset(0x1234, typeof(String), "myVar");

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))));
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_FailsWriteOffsetNoSourceData()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);
            fsuipcInterface.writeOffset(0x1234, typeof(String), "myVar");

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))));
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_FailsReadOffsetNotConnected()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            mockFsuipc.Setup(x => x.Open()).Throws(new Exception());

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);
            fsuipcInterface.readOffset(0x1234, typeof(Int32), "myVar");

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))));

            fsuipcInterface.shutdown();
        }

        [TestMethod]
        public void FSUIPCInterfaceTests_FailsReadOffsetIncorrectDataType()
        {
            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);
            fsuipcInterface.readOffset(0x1234, typeof(String), "myVar");

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))));
        }
    }
}
