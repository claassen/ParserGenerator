using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserGen.Generator;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace TesterCLI
{
    class Program
    {
        static void Main(string[] args)
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



            var parser = generator.GetParser(new List<string>() { "=", "var" });

            var tokens = parser.Parse(
                @"var x = 1;
                  var y = 2;
                  Test(x, y, z) 
                  {  
                      if(x == y) { return 0; }
                      else if(y == z) { return 1; }
                      else { return x + y + z; }
                      return 0;
                  } TestA(x, y) { return 0; }
                  var q = Test(x, y, z);"
            );
        }
    }
}
