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
    public class LL_k_Tests
    {
        [TestMethod]
        public void LL_k_GrammarTest()
        {
            var generator = new ParserGenerator(new List<string>()
            {
                "PROGRAM = EXPRESSION",
                "EXPRESSION = (X|Y)",
                "X = A B",
                "Y = A C",
                "A = 'a'",
                "B = 'b'",
                "C = 'c'"
            });

            var parser = generator.GetParser();

            var tokens = parser.Parse("a b");
            var tokens2 = parser.Parse("a c");
        }


        private LanguageParser GetCmParser()
        {
            var generator = new ParserGenerator(new List<string>()
            {
                "PROGRAM = (STATEMENT | FUNCTION_DEFINITION)+",
                "EXPRESSION = BOOLEAN_EXPRESSION",
                "STATEMENT = ((VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL|'return' EXPRESSION) ';' | CONDITIONAL)",
                "VARIABLE_DEFINITION = 'var' VARIABLE ('=' EXPRESSION)?",
                "ASSIGNMENT = VARIABLE '=' EXPRESSION",
                "FUNCTION_CALL = VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'",

                //"Precendence climbing method" of representing order of operations (http://en.wikipedia.org/wiki/Operator-precedence_parser)
                "BOOLEAN_EXPRESSION = EQUALITY_EXPRESSION (('&&'|'||') EQUALITY_EXPRESSION)*",
                "EQUALITY_EXPRESSION = ADDITIVE_EXPRESSION (('=='|'!='|'<'|'>'|'<='|'>=') ADDITIVE_EXPRESSION)*",
                "ADDITIVE_EXPRESSION = MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*",
                "MULTIPLICATIVE_EXPRESSION = PRIMARY (('*'|'/') PRIMARY)*",
                "PRIMARY = ('(' BOOLEAN_EXPRESSION ')' | NUMBER | FUNCTION_CALL | VARIABLE | '-' PRIMARY)",

                "REGEX:NUMBER = '[0-9]+'",
                "REGEX:VARIABLE = '[a-zA-Z]+'",

                "FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' FUNCTION_BODY",
                "FUNCTION_BODY = '{' (STATEMENT)* '}'",
            
                "CONDITIONAL = IF (ELSEIF)* (ELSE)?",
                "IF = 'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'",
                "ELSEIF = 'else' 'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'", 
                "ELSE = 'else' '{' (STATEMENT)* '}'"
            });

            return generator.GetParser();
        }

        [TestMethod]
        public void OperatorPrecedence_Test1()
        {
            var parser = GetCmParser();

            var tokens = parser.Parse(
                @"Add(x, y) 
                  {  
                      var test = 1 + 2 * 3 - 4 / 5;
                     
                      return (x + y) / 2; 
                  }"
            );
        }

        [TestMethod]
        public void OperatorPrecedence_Test2()
        {
            var parser = GetCmParser();

            var tokens = parser.Parse(
                @"Add(x, y) 
                  {  
                      var test = (1 + 2) * 3;

                      return (x + y) / 2; 
                  }"
            );
        }

        [TestMethod]
        public void OperatorPrecedence_Test3()
        {
            var parser = GetCmParser();

            var tokens = parser.Parse(
                @"var test = One() && Two();"
            );
        }

        [TestMethod]
        public void OperatorPrecedence_Test4()
        {
            var parser = GetCmParser();

            var tokens = parser.Parse(
                @"var test = 1 <= 2 != 3 > 4;"
            );
        }

        [TestMethod]
        public void MultipleStatements_Test()
        {
            var parser = GetCmParser();

            var tokens = parser.Parse(
                @"var a = (1 + 2 + 3) * 2;
                  var b = 2;
                  Add(x, y) 
                  {  
                      return x + y; 
                  }
                  Mult(x, y)
                  {
                      var temp = x * y;
                      return temp;
                  }
                  Avg(x, y, z)
                  {
                      return (x + y + z) * 2;
                  }
                  var x = Add(a, b);"
            );
        }
    }
}
