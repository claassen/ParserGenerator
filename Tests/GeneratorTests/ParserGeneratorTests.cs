using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;

namespace Tests.GeneratorTests
{
    [TestClass]
    public class ParserGeneratorTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParserGeneratorTest_InvalidExpression()
        {
            ParserGenerator generator = new ParserGenerator();

            generator.AddExpression("SOMETHING");
        }

        [TestMethod]
        public void Test()
        {
            ParserGen.Generator.GrammarParsing.GrammarParser parser = new ParserGen.Generator.GrammarParsing.GrammarParser();

            parser.Test("EXPRESSION = X '=' (A|B)");
        }
    }
}
