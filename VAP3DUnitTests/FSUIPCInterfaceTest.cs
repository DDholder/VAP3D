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

        public void ReadOffsetTest<T>(T variableValue, VADataTypes typeToSet)
        {
            const int address = 0x1234;
            const String destVar = "myVar";

            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            var mockedOffset = new Mock<IOffset<T>>();
            mockedOffset.Setup(x => x.GetValue()).Returns(variableValue);
            mockedOffset.Setup(x => x.GetUnderlyingType()).Returns(typeof(T));

            mockOffsetFactory.Setup(x => x.createOffsetForType(It.IsAny<int>(), It.IsAny<Type>())).Returns(
                mockedOffset.Object);

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            fsuipcInterface.readOffset(address, typeof(T), destVar);

            mockProxy.Verify(x => x.WriteToLog(It.IsAny<string>(), It.Is<string>(s => s.Equals("red"))), Times.Never);
            mockFsuipc.Verify(x => x.Process(), Times.Exactly(1));
            mockOffsetFactory.Verify(x => x.createOffsetForType(address, typeof(T)), Times.Exactly(1));
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
            ReadOffsetTest<Char>((char)50, VADataTypes.Int);
            ReadOffsetTest<Byte>(Byte.MaxValue, VADataTypes.Int);
            ReadOffsetTest<Int16>(5, VADataTypes.Int);
            ReadOffsetTest<UInt16>(UInt16.MaxValue, VADataTypes.Int);
            ReadOffsetTest<Int32>(Int32.MinValue, VADataTypes.Int);
            ReadOffsetTest<UInt32>(128, VADataTypes.Int);
            ReadOffsetTest<Int64>(Int32.MaxValue, VADataTypes.Int);
            ReadOffsetTest<UInt64>(50, VADataTypes.Int);

            // Decimal types
            ReadOffsetTest<Double>(10.4583, VADataTypes.Decimal);
            ReadOffsetTest<Single>(-1.0001f, VADataTypes.Decimal);

            // Boolean types
            ReadOffsetTest<Boolean>(true, VADataTypes.Bool);
            ReadOffsetTest<Boolean>(false, VADataTypes.Bool);
        }

        public void WriteOffsetTest<T>(object variableValue, VADataTypes typeToGet)
        {
            const int address = 0x1234;
            const String sourceVar = "mySourceVar";

            var mockFsuipc = new Mock<IFSUIPC>();
            var mockOffsetFactory = new Mock<IOffsetFactory>();
            var mockProxy = new Mock<MyVAProxy>();

            var mockedOffset = new Mock<IOffset<T>>();
            mockedOffset.Setup(x => x.GetUnderlyingType()).Returns(typeof(T));

            mockOffsetFactory.Setup(x => x.createOffsetForType(It.IsAny<int>(),
                It.IsAny<Type>(), true)).Returns(
                    mockedOffset.Object);

            if (typeToGet == VADataTypes.Int)
            {
                mockProxy.Setup(x => x.GetInt(It.IsAny<string>())).Returns(Convert.ToInt64(variableValue));
            }
            else if (typeToGet == VADataTypes.Decimal)
            {
                mockProxy.Setup(x => x.GetDecimal(It.IsAny<string>())).Returns(Convert.ToDecimal(variableValue));
            }
            else if (typeToGet == VADataTypes.Bool)
            {
                mockProxy.Setup(x => x.GetBoolean(It.IsAny<string>())).Returns(Convert.ToBoolean(variableValue));
            }

            FSUIPCInterface fsuipcInterface = new FSUIPCInterface(mockFsuipc.Object, mockOffsetFactory.Object);

            fsuipcInterface.initialise(mockProxy.Object);

            fsuipcInterface.writeOffset(address, typeof(T), sourceVar);

            mockFsuipc.Verify(x => x.Process(), Times.Exactly(1));
            mockOffsetFactory.Verify(x => x.createOffsetForType(address, typeof(T), true), Times.Exactly(1));

            mockedOffset.Verify(x => x.SetValue((T)Convert.ChangeType(variableValue, typeof(T))));

            if (typeToGet == VADataTypes.Int)
            {
                mockProxy.Verify(x => x.GetInt(sourceVar), Times.Once());
            }
            else if (typeToGet == VADataTypes.Decimal)
            {
                mockProxy.Verify(x => x.GetDecimal(sourceVar), Times.Once());
            }
            else if (typeToGet == VADataTypes.Bool)
            {
                mockProxy.Verify(x => x.GetBoolean(sourceVar), Times.Once());
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
            WriteOffsetTest<Char>((char)50, VADataTypes.Int);
            WriteOffsetTest<Byte>(Byte.MaxValue, VADataTypes.Int);
            WriteOffsetTest<Int16>(5, VADataTypes.Int);
            WriteOffsetTest<UInt16>(UInt16.MaxValue, VADataTypes.Int);
            WriteOffsetTest<Int32>(Int32.MinValue, VADataTypes.Int);
            WriteOffsetTest<UInt32>(128, VADataTypes.Int);
            WriteOffsetTest<Int64>(Int64.MaxValue, VADataTypes.Int);
            WriteOffsetTest<UInt64>(50, VADataTypes.Int);

            // Decimal types
            WriteOffsetTest<Double>(10.4583, VADataTypes.Decimal);
            WriteOffsetTest<Single>(-1.0001, VADataTypes.Decimal);

            // Boolean types
            WriteOffsetTest<Boolean>(true, VADataTypes.Bool);
            WriteOffsetTest<Boolean>(false, VADataTypes.Bool);
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
