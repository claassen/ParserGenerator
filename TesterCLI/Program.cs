using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator;

namespace TesterCLI
{
    class Program
    {
        static void Main(string[] args)
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
            
            generator.AddExpression("REGEX:NUMBER = '[0-9]+'");
            generator.AddExpression("REGEX:VARIABLE = '[a-zA-Z]+'");

            generator.AddExpression("FUNCTION_DEFINITION = VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' '{' FUNCTION_BODY '}'");
            generator.AddExpression("FUNCTION_BODY = (STATEMENT)* RETURN_STATEMENT");
            generator.AddExpression("RETURN_STATEMENT = 'return' EXPRESSION ';'");

            var parser = generator.GetParser();

            var tokens = parser.Parse(
                @"var a = 1;
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
                      return (x + y + z) / 3;
                  }"
            );
        }
    }
}
