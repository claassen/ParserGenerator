using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;
using ParserGen.Generator.GrammarParsing;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace Tests
{
    [TestClass]
    public class FunctionalLanguageTests
    {
        private readonly string[] expressions = new string[]
        {
            @"PROGRAM            = EXPR", 
            @"EXPR               = (EXPRESSION | '(' EXPR ')' )",
            @"EXPRESSION         = (VALUE | (MATH_EXPRESSION|LOGICAL_EXPRESSION|LAMBDA) )",
            @"VALUE              = (NUMBER|VARIABLE)",
            @"MATH_EXPRESSION    = MATH_OP EXPR EXPR",
            @"LOGICAL_EXPRESSION = LOGICAL_OP EXPR EXPR EXPR EXPR",
            @"LAMBDA             = '(' 'lambda' ARGS_LIST EXPR (EXPR)+ ')'",
            @"ARGS_LIST          = '(' VARIABLE (',' VARIABLE)* ')'",
            @"REGEX:VARIABLE     = '[a-zA-Z]+'",
            @"REGEX:NUMBER       = '[0-9]+'",
            @"MATH_OP            = ('+'|'-'|'*'|'/')",
            @"LOGICAL_OP         = ('<'|'>'|'<='|'>=')"
        };

        private LanguageParser CreateParser()
        {
            var generator = new ParserGenerator(expressions.ToList());

            return generator.GetParser();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FunctionalLanguageTest_MissingOpenParen()
        {
            var parser = CreateParser();

            parser.Parse("+ 1 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FunctionalLanguageTest_MissingCloseParen()
        {
            var parser = CreateParser();

            parser.Parse("(+ 1 2");
        }

        [TestMethod]
        public void FunctionalLanguageTest_ManyNestedParens_NoThrow()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(((+ (((((1))))) (((((((2))))))))))");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void FunctionalLanguageTest_ManyNestedParensMismatched()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(((+ (((((1))))) (((((((2)))))))))");
        }

        [TestMethod]
        public void FunctionalLanguageTest_SimpleMath_CheckAST()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("+ 1 2");

            var mathExpression = (DefaultLanguageNonTerminalToken)tokens[0];
            Assert.AreEqual("MATH_EXPRESSION", mathExpression.Name);

            var mathOpToken = (DefaultLanguageTerminalToken)mathExpression.Tokens[0];
            Assert.AreEqual("+", mathOpToken.Value);

            var operand1 = (DefaultLanguageTerminalToken)mathExpression.Tokens[1];
            Assert.AreEqual("NUMBER", operand1.Name);
            Assert.AreEqual("1", operand1.Value);

            var operand2 = (DefaultLanguageTerminalToken)mathExpression.Tokens[2];
            Assert.AreEqual("NUMBER", operand2.Name);
            Assert.AreEqual("2", operand2.Value);
        }

        [TestMethod]
        public void FunctionalLanguageTest_LambdaExpression_NoThrow()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(+ 1 (lambda (x,y) (+ x y) 1 (* 2 2)))");
        }
    }
}
