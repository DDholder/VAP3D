using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;
using Moq;

namespace VAP3DUnitTests
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void UtilitiesTest_GetsNumberOfBytesForSupportedTypes()
        {
            Assert.AreEqual(1, Utilities.numBytesFromType(typeof(Char)));
            Assert.AreEqual(1, Utilities.numBytesFromType(typeof(Byte)));
            Assert.AreEqual(2, Utilities.numBytesFromType(typeof(Int16)));
            Assert.AreEqual(2, Utilities.numBytesFromType(typeof(UInt16)));
            Assert.AreEqual(4, Utilities.numBytesFromType(typeof(Int32)));
            Assert.AreEqual(4, Utilities.numBytesFromType(typeof(UInt32)));
            Assert.AreEqual(8, Utilities.numBytesFromType(typeof(Int64)));
            Assert.AreEqual(8, Utilities.numBytesFromType(typeof(UInt64)));
            Assert.AreEqual(4, Utilities.numBytesFromType(typeof(Single)));
            Assert.AreEqual(8, Utilities.numBytesFromType(typeof(Double)));
            Assert.AreEqual(1, Utilities.numBytesFromType(typeof(Boolean)));

            Assert.AreEqual(-1, Utilities.numBytesFromType(typeof(String)));
            Assert.AreEqual(-1, Utilities.numBytesFromType(typeof(Decimal)));
        }

        [TestMethod]
        public void UtilitiesTest_SetsVAVariableFromOffset()
        {
            string varName = "MyVar";
            {
                decimal val = 4.5M;
                var mockProxy = new Mock<MyVAProxy>();
                var mockOffset = new Mock<IOffset>();
                mockOffset.Setup(x => x.GetValue(It.IsAny<Type>())).Returns(val);

                Utilities.setVariableValueFromOffset(typeof(Single), mockOffset.Object, 
                    varName, mockProxy.Object);

                Utilities.setVariableValueFromOffset(typeof(Double), mockOffset.Object,
                    varName, mockProxy.Object);

                mockProxy.Verify(x => x.SetDecimal(It.Is<string>(s => s.Equals(varName)),
                    It.Is<decimal>(d => d.Equals(val))), Times.Exactly(2));
            }

            {
                long val = 64L;
                var mockProxy = new Mock<MyVAProxy>();
                var mockOffset = new Mock<IOffset>();
                mockOffset.Setup(x => x.GetValue(It.IsAny<Type>())).Returns(val);

                Utilities.setVariableValueFromOffset(typeof(Char), mockOffset.Object,
                    varName, mockProxy.Object);
                Utilities.setVariableValueFromOffset(typeof(Byte), mockOffset.Object,
                    varName, mockProxy.Object);

                Utilities.setVariableValueFromOffset(typeof(Int16), mockOffset.Object,
                    varName, mockProxy.Object);
                Utilities.setVariableValueFromOffset(typeof(UInt16), mockOffset.Object,
                    varName, mockProxy.Object);

                Utilities.setVariableValueFromOffset(typeof(Int32), mockOffset.Object,
                    varName, mockProxy.Object);
                Utilities.setVariableValueFromOffset(typeof(UInt32), mockOffset.Object,
                    varName, mockProxy.Object);

                Utilities.setVariableValueFromOffset(typeof(Int64), mockOffset.Object,
                    varName, mockProxy.Object);
                Utilities.setVariableValueFromOffset(typeof(UInt64), mockOffset.Object,
                    varName, mockProxy.Object);

                mockProxy.Verify(x => x.SetInt(It.Is<string>(s => s.Equals(varName)),
                    It.Is<long>(d => d.Equals(val))), Times.Exactly(8));
            }
            {
                bool val = true;
                var mockProxy = new Mock<MyVAProxy>();
                var mockOffset = new Mock<IOffset>();
                mockOffset.Setup(x => x.GetValue(It.IsAny<Type>())).Returns(val);

                Utilities.setVariableValueFromOffset(typeof(Boolean), mockOffset.Object,
                    varName, mockProxy.Object);

                mockProxy.Verify(x => x.SetBoolean(It.Is<string>(s => s.Equals(varName)),
                    It.Is<bool>(d => d.Equals(val))));
            }

            {
                string val = "unsupported";
                var mockProxy = new Mock<MyVAProxy>();
                var mockOffset = new Mock<IOffset>();
                mockOffset.Setup(x => x.GetValue(It.IsAny<Type>())).Returns(val);

                Assert.AreEqual(false, Utilities.setVariableValueFromOffset(typeof(String), mockOffset.Object,
                    varName, mockProxy.Object));
            }
        }

        [TestMethod]
        public void UtilitiesTest_GetsVAVariableForOffset()
        {
            string varName = "MyVar";
            {
                decimal val = 4.5M;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetDecimal(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Single), 
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Double),
                    varName, mockProxy.Object));
            }

            {
                long val = 64L;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetInt(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Char),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Byte),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Int16),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(UInt16),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Int16),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(UInt16),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Int32),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(UInt32),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Int64),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(UInt64),
                    varName, mockProxy.Object));
            }

            {
                bool val = true;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetBoolean(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValueForOffset(typeof(Boolean),
                    varName, mockProxy.Object));
            }

            { 
                var mockProxy = new Mock<MyVAProxy>();

                Assert.AreEqual(null, Utilities.getVariableValueForOffset(typeof(String),
                    varName, mockProxy.Object));
            }
        }
    }
}
