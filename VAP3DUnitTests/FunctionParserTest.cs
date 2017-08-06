using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;

namespace VAP3DUnitTests
{
    [TestClass]
    public class FunctionParserTest
    {
        [TestMethod]
        public void FunctionParserTests_NoArgs()
        {
            string function = "beginMonitoringEvents:";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "beginMonitoringEvents");
            Assert.AreEqual(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void FunctionParserTests_MultipleArgs()
        {
            string function = "readOffset:ABCD;Int16;myVar";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "readOffset");
            Assert.AreEqual(parser.Arguments.Count, 3);
            Assert.AreEqual(parser.Arguments[0], 0xABCD);
            Assert.AreEqual(parser.Arguments[1], typeof(short));
            Assert.AreEqual(parser.Arguments[2], "myVar");
        }

        [TestMethod]
        public void FunctionParserTests_NoFunctionName()
        {
            string function = "foo;bar";
            FunctionParser parser = new FunctionParser();

            Assert.IsFalse(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, null);
            Assert.AreEqual(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void FunctionParserTests_TrailingDelim()
        {
            string function = "readOffset:ABCD;Int16;myVar;";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "readOffset");
            Assert.AreEqual(parser.Arguments.Count, 3);
            Assert.AreEqual(parser.Arguments[0], 0xABCD);
            Assert.AreEqual(parser.Arguments[1], typeof(short));
            Assert.AreEqual(parser.Arguments[2], "myVar");
        }

        [TestMethod]
        public void FunctionParserTests_InvalidCharactersInArgument()
        {
            string function = "MyFunc:an/arg";
            FunctionParser parser = new FunctionParser();

            Assert.IsFalse(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "MyFunc");
            Assert.AreEqual(parser.Arguments.Count, 0);
        }
    }
}
