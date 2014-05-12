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
            @"ARGS_LIST          = '(' VARIABLE (',' VARIABLE)* ')'",
            @"PROGRAM            = EXPRESSION",   
            @"EXPRESSION         = (VALUE|'(' (MATH_EXPRESSION|LOGICAL_EXPRESSION|LAMBDA) ')')",
            @"VALUE              = (NUMBER|VARIABLE)",
            @"MATH_EXPRESSION    = MATH_OP EXPRESSION EXPRESSION",
            @"LOGICAL_EXPRESSION = LOGICAL_OP EXPRESSION EXPRESSION EXPRESSION EXPRESSION",
            @"LAMBDA             = 'lambda' ARGS_LIST EXPRESSION (EXPRESSION)+",
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
        public void FunctionalLanguageTest_MissingOuterParens()
        {
            var parser = CreateParser();

            parser.Parse("+ 1 2");
        }

        [TestMethod]
        public void FunctionalLanguageTest_SimpleMath()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(+ 1 2)");

            var program = (ILanguageSubToken)tokens[0];
            Assert.AreEqual("PROGRAM", program.Name);

            var expression = (ILanguageSubToken)program.Tokens[0];
            Assert.AreEqual("EXPRESSION", expression.Name);

            var mathExpression = (ILanguageSubToken)expression.Tokens[0];
            Assert.AreEqual("MATH_EXPRESSION", mathExpression.Name);

            var mathOp = (ILanguageSubToken)mathExpression.Tokens[0];
            Assert.AreEqual("MATH_OP", mathOp.Name);

            var mathOpToken = mathOp.Tokens[0];
            Assert.AreEqual("+", mathOpToken.Value);

            var operand1 = (ILanguageSubToken)mathExpression.Tokens[1];
            Assert.AreEqual("EXPRESSION", operand1.Name);

            var operand1Value = (ILanguageSubToken)operand1.Tokens[0];
            Assert.AreEqual("VALUE", operand1Value.Name);

            var operand1ValueNumber = operand1Value.Tokens[0];
            Assert.AreEqual("NUMBER", operand1ValueNumber.Name);
            Assert.AreEqual("1", operand1ValueNumber.Value);

            var operand2 = (ILanguageSubToken)mathExpression.Tokens[2];
            Assert.AreEqual("EXPRESSION", operand1.Name);

            var operand2Value = (ILanguageSubToken)operand2.Tokens[0];
            Assert.AreEqual("VALUE", operand2Value.Name);

            var operand2ValueNumber = operand2Value.Tokens[0];
            Assert.AreEqual("NUMBER", operand2ValueNumber.Name);
            Assert.AreEqual("2", operand2ValueNumber.Value);
        }

        [TestMethod]
        public void FunctionalLanguageTest_LambdaExpression()
        {
            var parser = CreateParser();

            var tokens = parser.Parse("(+ 1 (lambda (x,y) (+ x y) 1 (* 2 2)))");
        }
    }
}
