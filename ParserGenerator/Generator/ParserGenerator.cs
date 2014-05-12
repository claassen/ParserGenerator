using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator.SyntaxParsing;
using ParserGen.Parse;
using ParserGen.Parse.Tokens;

namespace ParserGen.Generator
{
    public class ParserGenerator
    {
        private Dictionary<string, SyntaxExpression> _expressionTable = new Dictionary<string, SyntaxExpression>();
        private SyntaxParser _parser;

        public ParserGenerator()
        {
            _parser = new SyntaxParser();
        }

        public void AddExpression(string syntaxExpression)
        {
            SyntaxExpression expression = _parser.ParseSyntaxExpression(syntaxExpression);

            _expressionTable.Add(expression.Name, expression);
        }

        public Parser GetParser()
        {
            return GetParser(new DefaultLanguageTokenCreator());
        }

        public Parser GetParser(ILanguageTokenCreator tokenCreator)
        {
            ValidateSyntaxExpressions();

            return new Parser(_expressionTable, tokenCreator);
        }

        private void ValidateSyntaxExpressions()
        {
            Dictionary<string, SyntaxExpression> expressionFirstTokens = new Dictionary<string, SyntaxExpression>();

            foreach (var exp in _expressionTable)
            {
                ISyntaxToken firstToken = GetFirstSyntaxToken(exp.Value);

                if (!expressionFirstTokens.ContainsKey(firstToken.ToString()))
                {
                    expressionFirstTokens.Add(firstToken.ToString(), exp.Value);
                }
                else
                {
                    SyntaxExpression first = expressionFirstTokens[firstToken.ToString()];
                    throw new Exception("Invalid syntax description. The following syntax expression share the same first token: " + exp.Value.Name + " and " + first.Name);
                }
            }
        }

        private ISyntaxToken GetFirstSyntaxToken(SyntaxExpression expression)
        {
            if (expression is RegexExpression)
            {
                return new ExpressionSyntaxToken() { ExpressionName = expression.Name };
            }
            else
            {
                return expression.Tokens[0];
            }
        }
    }
}
