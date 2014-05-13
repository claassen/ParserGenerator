using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParserGen.Generator;
using ParserGen.Generator.SyntaxParsing;
using ParserGen.Parse;

namespace Tests
{
    [TestClass]
    public class FunctionalLanguageTests
    {
        private readonly string[] expressions = new string[]
        {
            @"PROGRAM            = EXPR", 
            @"EXPR               = (EXPRESSION | '(' EXPR ')' )",
            @"EXPRESSION         = (VALUE| (MATH_EXPRESSION|LOGICAL_EXPRESSION|LAMBDA) )",
            @"VALUE              = (NUMBER|VARIABLE)",
            @"MATH_EXPRESSION    = MATH_OP EXPR EXPR",
            @"LOGICAL_EXPRESSION = LOGICAL_OP EXPR EXPR EXPR EXPR",
            @"LAMBDA             = '(' 'lambda' ARGS_LIST EXPR (EXPR)+ ')'",
            @"ARGS_LIST          = '(' VARIABLE (',' VARIABLE)* ')'",
            @"REGEX:VARIABLE     = [a-zA-Z]+",
            @"REGEX:NUMBER       = [0-9]+",
            @"MATH_OP            = ('+'|'-'|'*'|'/')",
            @"LOGICAL_OP         = ('<'|'>'|'<='|'>=')"
        };

        private Parser CreateParser()
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

            var program = (ILanguageSubToken)tokens[0];
            Assert.AreEqual("PROGRAM", program.Name);

            var expr = (ILanguageSubToken)program.Tokens[0];
            Assert.AreEqual("EXPR", expr.Name);

            var expression = (ILanguageSubToken)expr.Tokens[0];
            Assert.AreEqual("EXPRESSION", expression.Name);

            var mathExpression = (ILanguageSubToken)expression.Tokens[0];
            Assert.AreEqual("MATH_EXPRESSION", mathExpression.Name);

            var mathOp = (ILanguageSubToken)mathExpression.Tokens[0];
            Assert.AreEqual("MATH_OP", mathOp.Name);

            var mathOpToken = mathOp.Tokens[0];
            Assert.AreEqual("+", mathOpToken.Value);

            var operand1 = (ILanguageSubToken)mathExpression.Tokens[1];
            Assert.AreEqual("EXPR", operand1.Name);

            var operand1Expression = (ILanguageSubToken)operand1.Tokens[0];
            Assert.AreEqual("EXPRESSION", operand1Expression.Name);

            var operand1Value = (ILanguageSubToken)operand1Expression.Tokens[0];
            Assert.AreEqual("VALUE", operand1Value.Name);

            var operand1ValueNumber = operand1Value.Tokens[0];
            Assert.AreEqual("NUMBER", operand1ValueNumber.Name);
            Assert.AreEqual("1", operand1ValueNumber.Value);

            var operand2 = (ILanguageSubToken)mathExpression.Tokens[2];
            Assert.AreEqual("EXPR", operand1.Name);

            var operand2Expression = (ILanguageSubToken)operand2.Tokens[0];
            Assert.AreEqual("EXPRESSION", operand2Expression.Name);

            var operand2Value = (ILanguageSubToken)operand2Expression.Tokens[0];
            Assert.AreEqual("VALUE", operand2Value.Name);

            var operand2ValueNumber = operand2Value.Tokens[0];
            Assert.AreEqual("NUMBER", operand2ValueNumber.Name);
            Assert.AreEqual("2", operand2ValueNumber.Value);
        }

        [TestMethod]
        public void FunctionalLanguageTest_LambdaExpression_NoThrow()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(+ 1 (lambda (x,y) (+ x y) 1 (* 2 2)))");
        }
    }
}
