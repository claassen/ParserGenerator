using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens;
using ParserGen.Parser;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage
{
    public class GPLInterpreter
    {
        public GrammarExpression InterpretGPLTokens(List<ILanguageToken> tokens)
        {
            return GrammarExpression(tokens);
        }

        private GrammarExpression GrammarExpression(List<ILanguageToken> tokens)
        {
            GrammarExpression grammarExpression;

            if (!(tokens[0] is GPLExpressionToken))
            {
                throw new Exception("Error. Expecting: EXPRESSION");
            }

            var expressionToken = (GPLExpressionToken)tokens[0];

            if (expressionToken.Tokens[0] is GPLIdentifierToken)
            {
                string expressionName = ((GPLIdentifierToken)expressionToken.Tokens[0]).ExpressionName;

                if (!(expressionToken.Tokens[1] is GPLLiteralToken) || ((GPLLiteralToken)expressionToken.Tokens[1]).Text != "=")
                {
                    throw new Exception("Error. Expecting: '='");
                }

                grammarExpression = new GrammarExpression() { Name = expressionName, Tokens = new List<IGrammarToken>() };

                if (!(expressionToken.Tokens[2] is GPLTokenListToken))
                {
                    throw new Exception("Error. Expecting TOKEN_LIST");
                }

                TokenList(((GPLTokenListToken)expressionToken.Tokens[2]).Tokens, grammarExpression.Tokens);
            }
            else if (expressionToken.Tokens[0] is GPLRegexIdentififerToken)
            {
                string expressionName = ((GPLRegexIdentififerToken)expressionToken.Tokens[0]).ExpressionName.Substring(6);

                if (!(expressionToken.Tokens[1] is GPLLiteralToken) || ((GPLLiteralToken)expressionToken.Tokens[1]).Text != "=")
                {
                    throw new Exception("Error. Expecting: '='");
                }

                if (!(expressionToken.Tokens[2] is GPLRegexExpressionToken))
                {
                    throw new Exception("Error. Expecting: regex expression");
                }

                string regex = ((GPLRegexExpressionToken)expressionToken.Tokens[2]).Expression.Trim(new char[] { '\'' });

                if (!regex.StartsWith("^")) regex = "^" + regex;
                if (!regex.EndsWith("$")) regex = regex + "$";

                grammarExpression = new RegexExpression() { Name = expressionName, Expression = regex };
            }
            else
            {
                throw new Exception("Error. Expecting: IDENTIFIER");
            }

            return grammarExpression;
        }

        private void TokenList(List<ILanguageToken> tokens, List<IGrammarToken> grammarTokens)
        {
            foreach (var token in tokens)
            {
                Token(token, grammarTokens);
            }
        }

        private void Token(ILanguageToken token, List<IGrammarToken> grammarTokens)
        {
            if (token is GPLExprNameToken)
            {
                ExpressionName((GPLExprNameToken)token, grammarTokens);
            }
            else if (token is GPLUserLiteralToken)
            {
                LiteralToken((GPLUserLiteralToken)token, grammarTokens);
            }
            else if (token is GPLGroupToken)
            {
                GroupToken((GPLGroupToken)token, grammarTokens);
            }
            else
            {
                throw new Exception("Error. Expected: EXPR_NAME or USER_LITERAL or GROUP_TOKEN");
            }
        }

        private void ExpressionName(GPLExprNameToken token, List<IGrammarToken> grammarTokens)
        {
            grammarTokens.Add(new ExpressionGrammarToken() { ExpressionName = token.ExpressionName });
        }

        private void LiteralToken(GPLUserLiteralToken token, List<IGrammarToken> grammarTokens)
        {
            grammarTokens.Add(new LiteralGrammarToken() { Text = token.Text.Trim(new char[] { '\'' }) });
        }

        private void GroupToken(GPLGroupToken token, List<IGrammarToken> grammarTokens)
        {
            Queue<ILanguageToken> tokens = new Queue<ILanguageToken>(token.Tokens);
            List<IGrammarToken> groupTokens = new List<IGrammarToken>();

            var tokenList = tokens.Dequeue();

            if (!(tokenList is GPLTokenListToken))
            {
                throw new Exception("Error. Expecting: TOKEN_LIST");
            }

            List<IGrammarToken> subGroupTokens = new List<IGrammarToken>();

            TokenList(((GPLTokenListToken)tokenList).Tokens, subGroupTokens);

            groupTokens.Add(new TokenListGrammarToken() { Tokens = subGroupTokens });

            while (true)
            {
                if (tokens.Count == 0)
                {
                    break;
                }

                var next = tokens.Peek();

                if (!(next is GPLLiteralToken))
                {
                    throw new Exception("Error. Expected: '|', '*', '+', or '?'");
                }

                if (((GPLLiteralToken)next).Text != "|")
                {
                    break;
                }

                //Consume '|'
                tokens.Dequeue();

                if (tokens.Count == 0)
                {
                    throw new Exception("Error. Expected: TOKEN_LIST");
                }

                tokenList = tokens.Dequeue();

                if (!(tokenList is GPLTokenListToken))
                {
                    throw new Exception("Error. Expecting: TOKEN_LIST");
                }

                subGroupTokens = new List<IGrammarToken>();

                TokenList(((GPLTokenListToken)tokenList).Tokens, subGroupTokens);

                groupTokens.Add(new TokenListGrammarToken() { Tokens = subGroupTokens });
            }

            TokenRepeatType repeatType = TokenRepeatType.Single;

            if (tokens.Count > 0)
            {
                var option = tokens.Dequeue();

                if (!(option is GPLLiteralToken))
                {
                    throw new Exception("Error. Expecting: '*' or '+' or '?'");
                }

                string optionChar = ((GPLLiteralToken)option).Text;

                if (optionChar == "*")
                {
                    repeatType = TokenRepeatType.ZeroOrMore;
                }
                else if (optionChar == "+")
                {
                    repeatType = TokenRepeatType.OneOrMore;
                }
                else if (optionChar == "?")
                {
                    repeatType = TokenRepeatType.Optional;
                }
                else
                {
                    throw new Exception("Error. Expecting: '*' or '+' or '?'");
                }
            }

            grammarTokens.Add(new GroupGrammarToken() { Tokens = groupTokens, RepeatType = repeatType });
        }
    }
}
