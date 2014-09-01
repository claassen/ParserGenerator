using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;
using ParserGen.Generator.GrammarParsing;
using ParserGen.Parser;
using ParserGen.Parser.Exceptions;
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
        [ExpectedException(typeof(InvalidSyntaxException))]
        public void FunctionalLanguageTest_MissingOpenParen()
        {
            var parser = CreateParser();

            parser.Parse("+ 1 2)");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSyntaxException))]
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
        [ExpectedException(typeof(InvalidSyntaxException))]
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

            var program = (DefaultLanguageNonTerminalToken)tokens[0];
            Assert.AreEqual("PROGRAM", program.Name);

            var expr = (DefaultLanguageNonTerminalToken)program.Tokens[0];
            Assert.AreEqual("EXPR", expr.Name);

            var expression = (DefaultLanguageNonTerminalToken)expr.Tokens[0];
            Assert.AreEqual("EXPRESSION", expression.Name);

            var mathExpression = (DefaultLanguageNonTerminalToken)expression.Tokens[0];
            Assert.AreEqual("MATH_EXPRESSION", mathExpression.Name);

            var mathOp = (DefaultLanguageNonTerminalToken)mathExpression.Tokens[0];
            Assert.AreEqual("MATH_OP", mathOp.Name);

            var expr1 = (DefaultLanguageNonTerminalToken)mathExpression.Tokens[1];
            Assert.AreEqual("EXPR", expr1.Name);

            var expr2 = (DefaultLanguageNonTerminalToken)mathExpression.Tokens[2];
            Assert.AreEqual("EXPR", expr2.Name);

            var plus = (DefaultLanguageTerminalToken)mathOp.Tokens[0];
            Assert.AreEqual("+", plus.Value);

            var oneExpr = (DefaultLanguageNonTerminalToken)expr1.Tokens[0];
            Assert.AreEqual("EXPRESSION", oneExpr.Name);

            var oneValue = (DefaultLanguageNonTerminalToken)oneExpr.Tokens[0];
            Assert.AreEqual("VALUE", oneValue.Name);

            var oneNumber = (DefaultLanguageTerminalToken)oneValue.Tokens[0];
            Assert.AreEqual("NUMBER", oneNumber.Name);
            Assert.AreEqual("1", oneNumber.Value);

            var twoExpr = (DefaultLanguageNonTerminalToken)expr2.Tokens[0];
            Assert.AreEqual("EXPRESSION", twoExpr.Name);

            var twoValue = (DefaultLanguageNonTerminalToken)twoExpr.Tokens[0];
            Assert.AreEqual("VALUE", twoValue.Name);

            var twoNumber = (DefaultLanguageTerminalToken)twoValue.Tokens[0];
            Assert.AreEqual("NUMBER", twoNumber.Name);
            Assert.AreEqual("2", twoNumber.Value);
        }

        [TestMethod]
        public void FunctionalLanguageTest_LambdaExpression_NoThrow()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(+ 1 (lambda (x,y) (+ x y) 1 (* 2 2)))");
        }
    }
}
