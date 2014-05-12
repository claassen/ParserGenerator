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
        public void ParserGeneratorTest_NonUniqueStartExpressions()
        {
            string expression1 = "'(' A ')'";
            string expression2 = "'(' B ')'";

            ParserGenerator generator = new ParserGenerator();

            generator.AddExpression(expression1);
            generator.AddExpression(expression2);
        }
    }
}
