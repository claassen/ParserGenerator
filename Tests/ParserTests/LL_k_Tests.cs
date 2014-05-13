using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;

namespace Tests.ParserTests
{
    [TestClass]
    public class LL_k_Tests
    {
        [TestMethod]
        public void LL_kTest()
        {
            var generator = new ParserGenerator();

            generator.AddExpression("PROGRAM = EXPRESSION");
            generator.AddExpression("EXPRESSION = (X|Y)");
            generator.AddExpression("X = A B");
            generator.AddExpression("Y = A C");
            generator.AddExpression("A = 'a'");
            generator.AddExpression("B = 'b'");
            generator.AddExpression("C = 'c'");

            var parser = generator.GetParser();

            var tokens = parser.Parse("a b");
            var tokens2 = parser.Parse("a c");
        }
    }
}
