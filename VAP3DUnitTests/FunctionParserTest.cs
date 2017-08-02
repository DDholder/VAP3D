using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VAP3D;

namespace VAP3DUnitTests
{
    [TestClass]
    public class FunctionParserTest
    {
        [TestMethod]
        public void ParseFunctionNoArgs()
        {
            string function = "beginMonitoringEvents:";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "beginMonitoringEvents");
            Assert.AreEqual(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void ParseFunctionMultipleArgs()
        {
            string function = "readOffset:ABCD;2;myVar";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "readOffset");
            Assert.AreEqual(parser.Arguments.Count, 3);
            Assert.AreEqual(parser.Arguments[0], 0xABCD);
            Assert.AreEqual(parser.Arguments[1], 2);
            Assert.AreEqual(parser.Arguments[2], "myVar");
        }

        [TestMethod]
        public void ParseFunctionNoFunctionName()
        {
            string function = "foo;bar";
            FunctionParser parser = new FunctionParser();

            Assert.IsFalse(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, null);
            Assert.AreEqual(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void ParseFunctionTrailingDelim()
        {
            string function = "readOffset:ABCD;2;myVar;";
            FunctionParser parser = new FunctionParser();

            Assert.IsTrue(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "readOffset");
            Assert.AreEqual(parser.Arguments.Count, 3);
            Assert.AreEqual(parser.Arguments[0], 0xABCD);
            Assert.AreEqual(parser.Arguments[1], 2);
            Assert.AreEqual(parser.Arguments[2], "myVar");
        }

        [TestMethod]
        public void ParseFunctionInvalidCharactersInArgument()
        {
            string function = "MyFunc:an/arg";
            FunctionParser parser = new FunctionParser();

            Assert.IsFalse(parser.parseFunction(function));

            Assert.AreEqual(parser.Function, "MyFunc");
            Assert.AreEqual(parser.Arguments.Count, 0);
        }
    }
}
