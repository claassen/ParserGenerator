using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;
using ParserGen.Parser;

namespace Tests.ParserTests
{
    [TestClass]
    public class ImperativeLanguageTests
    {
        private readonly string[] expressions = new string[]
        {
            @"PROGRAM      = 'BEGIN' (ASSIGNMENT)+ 'END'",
            @"ASSIGNMENT       = VARIABLE '=' VALUE ';'",
            @"VALUE              = (NUMBER|VARIABLE)",
            @"REGEX:VARIABLE     = '[a-zA-Z]+'",
            @"REGEX:NUMBER       = '[0-9]+'"
        };

        private LanguageParser CreateParser()
        {
            ParserGenerator generator = new ParserGenerator();

            foreach (string expression in expressions)
            {
                generator.AddExpression(expression);
            }

            return generator.GetParser();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ImperativeLanguageTest_MissingSemicolon()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("BEGIN x = 1; y = 2 END");
        }

        [TestMethod]
        public void ImperativeLanguageTest_Valid_NoThrow()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("BEGIN x = 1; y = 2; END");
        }
    }
}
