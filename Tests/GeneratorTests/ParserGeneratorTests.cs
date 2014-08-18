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
            var generator = new ParserGenerator(new List<string>()
            {
                "SOMETHING"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParserGeneratorTest_LeftRecursion_Fail()
        {
            var generator = new ParserGenerator(new List<string>()
            {
                "EXPRESSION = (EXPRESSION | SOMETHING)"
            });

            var parser = generator.GetParser();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParserGeneratorTest_IndirectLeftRecursion_Fail()
        {
            var generator = new ParserGenerator(new List<string>()
            {
                "PROGRAM = (STATEMENT | FUNCTION_DEFINITION)+",
                "EXPRESSION = (MATH_EXPRESSION | FUNCTION_CALL | NUMBER | VARIABLE)",
                "STATEMENT = (VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL) ';'",

                "VARIABLE_DEFINITION = 'var' VARIABLE ('=' EXPRESSION)?",
                "ASSIGNMENT = VARIABLE '=' EXPRESSION",
                "FUNCTION_CALL = VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'",

                //"Precendence climbing method" of representing order of operations (http://en.wikipedia.org/wiki/Operator-precedence_parser)
                //"MATH_EXPRESSION = EQUALITY_EXPRESSION",
                //"EQUALITY_EXPRESSION = ADDITIVE_EXPRESSION (('=='|'!=') ADDITIVE_EXPRESSION)*",
                //"ADDITIVE_EXPRESSION = MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*",
                //"MULTIPLICATIVE_EXPRESSION = PRIMARY (('*'|'/') PRIMARY)*",
                //"PRIMARY = '(' MATH_EXPRESSION ')' | NUMBER | VARIABLE | '-' PRIMARY",
                "MATH_EXPRESSION = EXPRESSION MATH_OP EXPRESSION",
                "MATH_OP = ('+' | '-' | '*' | '/')",

                "REGEX:NUMBER = '[0-9]+'",
                "REGEX:VARIABLE = '[a-zA-Z]+'",

                "FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' '{' FUNCTION_BODY '}'",
                "FUNCTION_BODY = (STATEMENT)* RETURN_STATEMENT",
                "RETURN_STATEMENT = 'return' EXPRESSION ';'"
            });

            var parser = generator.GetParser();
        }

        [TestMethod]
        public void ParserGeneratorTest_IndirectLeftRecursion_Fixed()
        {
            var generator = new ParserGenerator(new List<string>()
            {
                "PROGRAM = (STATEMENT | FUNCTION_DEFINITION)+",
                "EXPRESSION = (MATH_EXPRESSION | FUNCTION_CALL | NUMBER | VARIABLE)",
                "STATEMENT = (VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL) ';'",

                "VARIABLE_DEFINITION = 'var' VARIABLE ('=' EXPRESSION)?",
                "ASSIGNMENT = VARIABLE '=' EXPRESSION",
                "FUNCTION_CALL = VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'",

                //"Precendence climbing method" of representing order of operations (http://en.wikipedia.org/wiki/Operator-precedence_parser)
                "MATH_EXPRESSION = EQUALITY_EXPRESSION",
                "EQUALITY_EXPRESSION = ADDITIVE_EXPRESSION (('=='|'!=') ADDITIVE_EXPRESSION)?",
                "ADDITIVE_EXPRESSION = MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*",
                "MULTIPLICATIVE_EXPRESSION = PRIMARY (('*'|'/') PRIMARY)*",
                "PRIMARY = ('(' MATH_EXPRESSION ')' | NUMBER | VARIABLE | '-' PRIMARY)",
                //"MATH_EXPRESSION = EXPRESSION MATH_OP EXPRESSION",
                //"MATH_OP = ('+'|'-'|'*'|'/')",

                "REGEX:NUMBER = '[0-9]+'",
                "REGEX:VARIABLE = '[a-zA-Z]+'",

                "FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' '{' FUNCTION_BODY '}'",
                "FUNCTION_BODY = (STATEMENT)* RETURN_STATEMENT",
                "RETURN_STATEMENT = 'return' EXPRESSION ';'"
            });

            var parser = generator.GetParser();
        }
    }
}
