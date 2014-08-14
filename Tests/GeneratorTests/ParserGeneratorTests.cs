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
        public void ParserGeneratorTest_InvalidExpression_Fail()
        {
            ParserGenerator generator = new ParserGenerator();

            generator.AddExpression("SOMETHING");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParserGeneratorTest_LeftRecursion_Fail()
        {
            ParserGenerator generator = new ParserGenerator();

            generator.AddExpression("EXPRESSION = EXPRESSION | SOMETHING");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParserGeneratorTest_IndirectLeftRecursion_Fail()
        {
            var generator = new ParserGenerator();

            generator.AddExpression("PROGRAM = (STATEMENT | FUNCTION_DEFINITION)+");
            generator.AddExpression("EXPRESSION = (MATH_EXPRESSION | FUNCTION_CALL | NUMBER | VARIABLE)");
            generator.AddExpression("STATEMENT = (VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL) ';'");

            generator.AddExpression("VARIABLE_DEFINITION = 'var' VARIABLE ('=' EXPRESSION)?");
            generator.AddExpression("ASSIGNMENT = VARIABLE '=' EXPRESSION");
            generator.AddExpression("FUNCTION_CALL = VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'");

            //"Precendence climbing method" of representing order of operations (http://en.wikipedia.org/wiki/Operator-precedence_parser)
            //generator.AddExpression("MATH_EXPRESSION = EQUALITY_EXPRESSION");
            //generator.AddExpression("EQUALITY_EXPRESSION = ADDITIVE_EXPRESSION (('=='|'!=') ADDITIVE_EXPRESSION)*");
            //generator.AddExpression("ADDITIVE_EXPRESSION = MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*");
            //generator.AddExpression("MULTIPLICATIVE_EXPRESSION = PRIMARY (('*'|'/') PRIMARY)*");
            //generator.AddExpression("PRIMARY = '(' MATH_EXPRESSION ')' | NUMBER | VARIABLE | '-' PRIMARY");
            generator.AddExpression("MATH_EXPRESSION = EXPRESSION MATH_OP EXPRESSION");
            generator.AddExpression("MATH_OP = ('+'|'-'|'*'|'/')");

            generator.AddExpression("REGEX:NUMBER = '[0-9]+'");
            generator.AddExpression("REGEX:VARIABLE = '[a-zA-Z]+'");

            generator.AddExpression("FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' '{' FUNCTION_BODY '}'");
            generator.AddExpression("FUNCTION_BODY = (STATEMENT)* RETURN_STATEMENT");
            generator.AddExpression("RETURN_STATEMENT = 'return' EXPRESSION ';'");

            var parser = generator.GetParser();
        }

        [TestMethod]
        public void ParserGeneratorTest_IndirectLeftRecursion_Fixed()
        {
            var generator = new ParserGenerator();

            generator.AddExpression("PROGRAM = (STATEMENT | FUNCTION_DEFINITION)+");
            generator.AddExpression("EXPRESSION = (MATH_EXPRESSION | FUNCTION_CALL | NUMBER | VARIABLE)");
            generator.AddExpression("STATEMENT = (VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL) ';'");

            generator.AddExpression("VARIABLE_DEFINITION = 'var' VARIABLE ('=' EXPRESSION)?");
            generator.AddExpression("ASSIGNMENT = VARIABLE '=' EXPRESSION");
            generator.AddExpression("FUNCTION_CALL = VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'");

            //"Precendence climbing method" of representing order of operations (http://en.wikipedia.org/wiki/Operator-precedence_parser)
            generator.AddExpression("MATH_EXPRESSION = EQUALITY_EXPRESSION");
            generator.AddExpression("EQUALITY_EXPRESSION = ADDITIVE_EXPRESSION (('=='|'!=') ADDITIVE_EXPRESSION)*");
            generator.AddExpression("ADDITIVE_EXPRESSION = MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*");
            generator.AddExpression("MULTIPLICATIVE_EXPRESSION = PRIMARY (('*'|'/') PRIMARY)*");
            generator.AddExpression("PRIMARY = ('(' MATH_EXPRESSION ')' | NUMBER | VARIABLE | '-' PRIMARY)");
            //generator.AddExpression("MATH_EXPRESSION = EXPRESSION MATH_OP EXPRESSION");
            //generator.AddExpression("MATH_OP = ('+'|'-'|'*'|'/')");

            generator.AddExpression("REGEX:NUMBER = '[0-9]+'");
            generator.AddExpression("REGEX:VARIABLE = '[a-zA-Z]+'");

            generator.AddExpression("FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' '{' FUNCTION_BODY '}'");
            generator.AddExpression("FUNCTION_BODY = (STATEMENT)* RETURN_STATEMENT");
            generator.AddExpression("RETURN_STATEMENT = 'return' EXPRESSION ';'");

            var parser = generator.GetParser();
        }
    }
}
