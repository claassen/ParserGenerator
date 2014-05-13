using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserGen.Generator.SyntaxParsing;

namespace ParserGen.Parse
{
    public class Parser
    {
        private Dictionary<string, SyntaxExpression> _expressionTable;
        private ILanguageTokenCreator _tokenCreator;
        private HashSet<string> _reservedWords;
        
        private string _input;
        private string _currentToken;

        public Parser(Dictionary<string, SyntaxExpression> expressionTable, ILanguageTokenCreator tokenCreator)
        {
            _expressionTable = expressionTable;
            _tokenCreator = tokenCreator;

            //Uses all literal tokens present in the given syntax as reserved keywords so that user supplied
            //regex expressions won't match language keywords or special purpose characters
            _reservedWords = new HashSet<string>(
                _expressionTable.Where(i => i.Value.Tokens != null).SelectMany(
                    i => i.Value.Tokens.Where(t => t != null && t is LiteralSyntaxToken).Select(t => ((LiteralSyntaxToken)t).Text)
                )
            );
        }

        private void Scan()
        {
            string[] tokens = Regex.Split(_input, @"(?=[ \(\)\|\[\];,])|(?<=[ \(\)\|\[\];,])");

            _currentToken = tokens.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            if (!string.IsNullOrEmpty(_currentToken))
            {
                _input = _input.Substring(_currentToken.Length).TrimStart();
            }
        }

        public List<ILanguageToken> Parse(string source)
        {
            _input = source;

            List<ILanguageToken> tokens = new List<ILanguageToken>();

            string ROOT_EXPRESSION = _tokenCreator.RootExpressionName();

            Scan();

            ParseSyntaxExpression(_expressionTable[ROOT_EXPRESSION], tokens);

            if (_currentToken != null)
            {
                throw new Exception("Syntax error. Unexpected token: " + _currentToken);
            }

            return tokens;
        }

        private void ParseSyntaxExpression(SyntaxExpression expression, List<ILanguageToken> tokens)
        {
            if (expression is RegexExpression)
            {
                if (_currentToken != null &&
                    Regex.IsMatch(_currentToken, ((RegexExpression)expression).Expression) &&
                    !_reservedWords.Contains(_currentToken))
                {
                    tokens.Add(_tokenCreator.Create(expression.Name, _currentToken));
                    Scan();
                }
                else
                {
                    throw new Exception("Syntax error. Expecting: " + ((RegexExpression)expression).Name);
                }
            }
            else
            {
                List<ILanguageToken> subTokens = new List<ILanguageToken>();
                
                foreach (ISyntaxToken exprToken in expression.Tokens)
                {
                    if (!IsMatch(exprToken))
                    {
                        throw new Exception("Syntax error. Expecting: " + exprToken.ToString());
                    }

                    ParseSyntaxToken(exprToken, subTokens);
                }

                tokens.Add(new ILanguageSubToken() { Name = expression.Name, Tokens = subTokens });
            }
        }

        private void ParseSyntaxToken(ISyntaxToken exprToken, List<ILanguageToken> tokens)
        {
            if (exprToken is SubSyntaxToken)
            {
                ParseSubSyntaxToken((SubSyntaxToken)exprToken, tokens);
            }
            else if (exprToken is ExpressionSyntaxToken)
            {
                ParseExpressionSyntaxToken((ExpressionSyntaxToken)exprToken, tokens);
            }
            else if (exprToken is LiteralSyntaxToken)
            {
                ParseLiteralSyntaxToken((LiteralSyntaxToken)exprToken, tokens);
            }
            else if (exprToken is MultipleOptionSyntaxToken)
            {
                ParseMultipleOptionSyntaxToken((MultipleOptionSyntaxToken)exprToken, tokens);
            }
        }

        private void ParseSubSyntaxToken(SubSyntaxToken exprToken, List<ILanguageToken> tokens)
        {
            foreach (ISyntaxToken t in exprToken.Tokens)
            {
                if (!IsMatch(t))
                {
                    if (exprToken.IsOptional)
                    {
                        break;
                    }
                    else
                    {
                        throw new Exception("Syntax error. Expecting: " + t.ToString());
                    }
                }

                ParseSyntaxToken(t, tokens);
            }
        }

        private void ParseExpressionSyntaxToken(ExpressionSyntaxToken exprToken, List<ILanguageToken> tokens)
        {
            SyntaxExpression expression = _expressionTable[exprToken.ExpressionName];

            ParseSyntaxExpression(expression, tokens);
        }

        private void ParseLiteralSyntaxToken(LiteralSyntaxToken exprToken, List<ILanguageToken> tokens)
        {
            if (!IsMatch(exprToken))
            {
                throw new Exception("Syntax error. Expecting: " + exprToken.Text);
            }

            //TODO: let user provided TokenCreator decide what to exclude
            if (_currentToken != "(" && _currentToken != ")" && _currentToken != ",")
            {
                tokens.Add(_tokenCreator.Create("LiteralToken", _currentToken));
            }
            Scan();
        }

        private void ParseMultipleOptionSyntaxToken(MultipleOptionSyntaxToken exprToken, List<ILanguageToken> tokens)
        {
            bool globalSuccess = false;

            string SAVED_INPUT = _input;
            string SAVED_TOKEN = _currentToken;

            while (true)
            {
                bool success = false;

                foreach (ISyntaxToken t in exprToken.Tokens)
                {
                    if (IsMatch(t))
                    {
                        var matchTokens = new List<ILanguageToken>();
                        try
                        {
                            ParseSyntaxToken(t, matchTokens);
                            tokens.AddRange(matchTokens);
                            success = true;
                            globalSuccess = true;
                            break;
                        }
                        catch
                        {
                            //Backtrack
                            _input = SAVED_INPUT;
                            _currentToken = SAVED_TOKEN;
                        }
                    }
                }

                if (!success || (exprToken.RepeatType != TokenRepeatType.ZeroOrMore && exprToken.RepeatType != TokenRepeatType.OneOrMore))
                {
                    break;
                }
            }

            if (!globalSuccess && exprToken.RepeatType != TokenRepeatType.ZeroOrMore)
            {
                throw new Exception("Syntax error. Expecting: " + exprToken.ToString());
            }
        }

        private bool IsMatch(ISyntaxToken token)
        {
            if (token is SubSyntaxToken)
            {
                return IsMatch(((SubSyntaxToken)token).Tokens[0]);
            }
            else if (token is ExpressionSyntaxToken)
            {
                SyntaxExpression expression = _expressionTable[((ExpressionSyntaxToken)token).ExpressionName];

                if (expression is RegexExpression)
                {
                    return _currentToken != null &&
                    Regex.IsMatch(_currentToken, ((RegexExpression)expression).Expression) &&
                    !_reservedWords.Contains(_currentToken);
                }
                else
                {
                    return IsMatch(expression.Tokens[0]);
                }
            }
            else if(token is LiteralSyntaxToken)
            {
                return _currentToken == ((LiteralSyntaxToken)token).Text;
            }
            else if(token is MultipleOptionSyntaxToken)
            {
                bool match = false;

                foreach (ISyntaxToken t in ((MultipleOptionSyntaxToken)token).Tokens)
                {
                    if (IsMatch(t))
                    {
                        match = true;
                        break;
                    }
                }

                return match;
            }

            throw new Exception("Unknown SyntaxToken type");
        }
    }
}
