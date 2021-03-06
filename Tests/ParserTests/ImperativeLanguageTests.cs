﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;
using ParserGen.Parser;
using ParserGen.Parser.Exceptions;

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
            var generator = new ParserGenerator(expressions.ToList());

            return generator.GetParser();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException))]
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
