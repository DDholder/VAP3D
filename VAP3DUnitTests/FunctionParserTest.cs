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
            string function = "MyFunc:";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, "MyFunc");
            Assert.Equals(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void ParseFunction1Arg()
        {
            string function = "MyFunc:someValue";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, "MyFunc");
            Assert.Equals(parser.Arguments.Count, 1);
            Assert.Equals(parser.Arguments[0], "someValue");
        }

        [TestMethod]
        public void ParseFunctionMultipleArgs()
        {
            string function = "MyFunc:someValue;1;true";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, "MyFunc");
            Assert.Equals(parser.Arguments.Count, 3);
            Assert.Equals(parser.Arguments[0], "someValue");
            Assert.Equals(parser.Arguments[1], "1");
            Assert.Equals(parser.Arguments[2], "true");
        }

        [TestMethod]
        public void ParseFunctionNoFunctionName()
        {
            string function = "foo;bar";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, null);
            Assert.Equals(parser.Arguments.Count, 0);
        }

        [TestMethod]
        public void ParseFunctionTrailingDelim()
        {
            string function = "Foo:bar;baz;";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, "Foo");
            Assert.Equals(parser.Arguments.Count, 2);
            Assert.Equals(parser.Arguments[0], "bar");
            Assert.Equals(parser.Arguments[1], "baz");
        }

        [TestMethod]
        public void ParseFunctionInvalidCharactersInArgument()
        {
            string function = "MyFunc:an/arg";
            FunctionParser parser = new FunctionParser(function);

            Assert.Equals(parser.Function, "MyFunc");
            Assert.Equals(parser.Arguments.Count, 0);
        }
    }
}
