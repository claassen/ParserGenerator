using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens
{
    public class GPLTokenCreator : ILanguageTokenCreator
    {
        public string RootExpressionName()
        {
            return "EXPRESSION";
        }

        public ILanguageToken Create(string literalToken)
        {
            return new GPLLiteralToken() { Text = literalToken };
        }

        public ILanguageToken Create(string expressionName, string expressionValue)
        {
            switch (expressionName)
            {
                case "IDENTIFIER":
                    return new GPLIdentifierToken() { ExpressionName = expressionValue };
                case "REGEX_IDENTIFIER":
                    return new GPLRegexIdentififerToken() { ExpressionName = expressionValue };
                case "EXPR_NAME":
                    return new GPLExprNameToken() { ExpressionName = expressionValue };   
                case "LITERAL_TOKEN":
                    return new GPLUserLiteralToken() { Text = expressionValue };
                case "REGEX_EXPRESSION":
                    string regex = expressionValue;
                    if (!regex.StartsWith("^")) regex = "^" + regex;
                    if (!regex.EndsWith("$")) regex = regex + "$";
                    return new GPLRegexExpressionToken() { Expression = expressionValue };
            }

            return null;
            //return base.Create(expressionName, expressionValue);
        }

        public ILanguageToken Create(string expressionName, List<ILanguageToken> tokens)
        {
            switch (expressionName)
            {
                case "EXPRESSION":
                    return new GPLExpressionToken() { ExpressionName = expressionName, Tokens = tokens };
                case "TOKEN_LIST":
                    return new GPLTokenListToken() { Tokens = tokens };
                case "GROUP_TOKEN":
                    return new GPLGroupToken() { Tokens = tokens };
                case "TOKEN":
                    return tokens[0];
            }

            return null;
            //return base.Create(expressionName, tokens);
        }
    }
}
