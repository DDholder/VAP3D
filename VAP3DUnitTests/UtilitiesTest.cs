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
        public void UtilitiesTest_GetsVAVariableForOffset()
        {
            string varName = "MyVar";
            {
                decimal val = 4.5M;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetDecimal(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Single), 
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Double),
                    varName, mockProxy.Object));
            }

            {
                long val = 64L;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetInt(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Char),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Byte),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Int16),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValue(typeof(UInt16),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Int16),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValue(typeof(UInt16),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Int32),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValue(typeof(UInt32),
                    varName, mockProxy.Object));

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Int64),
                    varName, mockProxy.Object));
                Assert.AreEqual(val, Utilities.getVariableValue(typeof(UInt64),
                    varName, mockProxy.Object));
            }

            {
                bool val = true;
                var mockProxy = new Mock<MyVAProxy>();
                mockProxy.Setup(x => x.GetBoolean(It.IsAny<string>())).Returns(val);

                Assert.AreEqual(val, Utilities.getVariableValue(typeof(Boolean),
                    varName, mockProxy.Object));
            }

            { 
                var mockProxy = new Mock<MyVAProxy>();

                Assert.AreEqual(null, Utilities.getVariableValue(typeof(String),
                    varName, mockProxy.Object));
            }
        }
    }
}
