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

            Test(tokens);
        }

        static Dictionary<string, int>[] SymbolTables = new Dictionary<string, int>[100];
        static int scopeLevel = 0;

        static void Test(List<ILanguageToken> tokens)
        {
            foreach (var token in tokens)
            {
                //if (token is DefaultLanguageNonTerminalToken)
                //{
                //    tab++;

                //    Console.WriteLine(Tab() + "Scope: " + (token as DefaultLanguageNonTerminalToken).Name);
                //    Test(((DefaultLanguageNonTerminalToken)token).Tokens);

                //    Emit(token as DefaultLanguageNonTerminalToken);                    

                //    tab--;
                //}
                //else
                //{
                //    string value = (token as DefaultLanguageToken).Value;

                //    if (value == "{")
                //    {
                //        Console.WriteLine("Enter new scope");
                //        CreateScope();
                //    }
                //    else if (value == "}")
                //    {
                //        Console.WriteLine("Exit scope");
                //        ExitScope();
                //    }
                //    Emit(token as DefaultLanguageToken);
                //}
            }
        }

        static void CreateScope()
        {
            scopeLevel++;
            SymbolTables[scopeLevel] = new Dictionary<string, int>();
        }

        static void ExitScope()
        {
            scopeLevel--;
        }

        static int tab = 0;

        //static void EmitMathExpression(ILanguageToken token)
        //{
        //    if (token is DefaultLanguageNonTerminalToken)
        //    {
        //        EmitMathExpression((token as DefaultLanguageNonTerminalToken).Tokens.First());
        //        EmitMathExpression((token as DefaultLanguageNonTerminalToken).Tokens.Last());
        //        Console.WriteLine("pop top 2");
        //        Console.WriteLine("perform " +((token as DefaultLanguageNonTerminalToken).Tokens[1] as DefaultLanguageToken).Value);
        //        Console.WriteLine("push result");
        //    }
        //    else
        //    {
        //        Console.WriteLine("push " + (token as DefaultLanguageToken).Value);
        //    }
        //}

        //static void Emit(DefaultLanguageToken token)
        //{
        //    Console.WriteLine(Tab() + "Evaluate: " + token.Name + " = " + token.Value);

        //    if (token.Name == "NUMBER")
        //    {
        //        //Console.WriteLine("mov " + token.Value + ", eax");
        //        //Console.WriteLine("push eax");
        //    }
        //    else if (token.Name == "VARIABLE")
        //    {
        //        //Console.WriteLine("load [" + token.Value + "], eax");
        //        //Console.WriteLine("push eax");
        //    }
        //}

        static void Emit(DefaultLanguageNonTerminalToken token)
        {
            Console.WriteLine(Tab() + "Evaluate: " + token.Name);   
        }

        static string Tab()
        {
            var sb = new StringBuilder();
            sb.Insert(0, " ", tab);
            return sb.ToString();
        }
    }

    
    public class StatementToken : ILanguageToken
    {
    }

    public class VariableDefinitionToken : StatementToken
    {
    }

    public class AssignmentToken : StatementToken
    {
    }

    public class FunctionCallToken : StatementToken
    {
        public string FunctionName { get; set; }
        public List<ILanguageToken> Arguments { get; set; }
    }

    public class ExpressionToken : ILanguageToken
    {
    }

    
    public class OperatorToken : ILanguageToken
    {
        public string Operator { get; set; }
    }

    public class FunctionToken : ILanguageToken
    {
        public List<ParameterToken> Parameters { get; set; }
        public List<StatementToken> Body { get; set; }
        public ExpressionToken ReturnStatement { get; set; }
    }

    public class ParameterToken : ILanguageToken
    {

    }
}
