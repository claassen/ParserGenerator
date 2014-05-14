using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens
{
    public class GrammarParserTokenCreator : ILanguageTokenCreator
    {
        public string RootExpressionName()
        {
            return "EXPRESSION";
        }

        public ILanguageToken Create(string literalToken)
        {
            return new LiteralToken() { Text = literalToken };
        }

        public ILanguageToken Create(string expressionName, string expressionValue)
        {
            switch (expressionName)
            {
                case "IDENTIFIER":
                    return new IdentifierToken() { ExpressionName = expressionValue };
                case "EXPR_NAME":
                    return new ExprNameToken() { ExpressionName = expressionValue };   
            }

            return null;
            //return base.Create(expressionName, expressionValue);
        }

        public ILanguageToken Create(string expressionName, List<ILanguageToken> tokens)
        {
            switch (expressionName)
            {
                case "EXPRESSION":
                    return new ExpressionToken() { ExpressionName = expressionName, Tokens = tokens };
                case "TOKEN_LIST":
                    return new TokenListToken() { Tokens = tokens };
                case "GROUP_TOKEN":
                    return new GroupToken() { Tokens = tokens };
                case "TOKEN":
                    return tokens[0];
            }

            return null;
            //return base.Create(expressionName, tokens);
        }
    }
}
