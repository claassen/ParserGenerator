using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserGen.Generator.SyntaxParsing
{
    public class SyntaxParser
    {
        private string _input;
        private string _currentToken;

        public SyntaxExpression ParseSyntaxExpression(string input)
        {
            _input = input;
            _currentToken = null;

            var tokens = new List<ISyntaxToken>();

            //Scan expession name
            Scan();
            
            string expressionName = _currentToken;
            bool isRegexExpression = false;

            if (expressionName.StartsWith("REGEX:"))
            {
                isRegexExpression = true;
                expressionName = expressionName.Substring(6);

                if (!_currentToken.StartsWith("^"))
                {
                    _currentToken = "^" + _currentToken;
                }

                if (!_currentToken.EndsWith("$"))
                {
                    _currentToken = _currentToken + "$";
                }
            }

            //Scan =
            Scan();

            if (_currentToken != "=")
            {
                throw new Exception("Invalid expression declaration string");
            }

            //Scan first token
            Scan();

            //If the expression is a regex expression (indicated by the name starting with REGEX:),
            //then we just take the literal string value of the expression as its
            if (isRegexExpression)
            {
                return new RegexExpression() { Name = expressionName, Expression = _currentToken + _input };
            }

            var e = new SyntaxExpression() { Name = expressionName };

            if (SyntaxExpression(tokens))
            {
                //success
                e.Tokens = tokens;
                return e;
            }
            else
            {
                return null;
            }
        }

        private void Scan()
        {
            string[] tokens = Regex.Split(_input, @"\s*('[()]'|[()]|[[\]]|[|])\s*|[\s[\]|]");

            _currentToken = tokens.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            if (!string.IsNullOrEmpty(_currentToken))
            {
                _input = _input.Substring(_currentToken.Length).TrimStart();
            }
        }

        private bool IsMatch(string token)
        {
            if (_currentToken == token)
            {
                Scan();
                return true;
            }
            return false;
        }

        private bool SyntaxExpression(List<ISyntaxToken> tokens)
        {
            while (true)
            {
                bool success = false;

                if (ExpressionSyntaxToken(tokens))
                {
                    success = true;
                }
                else if (LiteralSyntaxToken(tokens))
                {
                    success = true;
                }
                else if (OptionalSyntaxToken(tokens))
                {
                    success = true;
                }
                else if (MultipleOptionSyntaxToken(tokens))
                {
                    success = true;
                }

                if (!success)
                {
                    break;
                }
            }

            return true;
        }

        private bool ExpressionSyntaxToken(List<ISyntaxToken> tokens)
        {
            if (_currentToken != null && Regex.IsMatch(_currentToken, @"^[A-Z_]+$"))
            {
                tokens.Add(new ExpressionSyntaxToken() { ExpressionName = _currentToken });
                Scan();
                return true;
            }
            return false;
        }

        private bool LiteralSyntaxToken(List<ISyntaxToken> tokens)
        {
            if (_currentToken != null && Regex.IsMatch(_currentToken, @"'..*'"))
            {
                tokens.Add(new LiteralSyntaxToken() { Text = _currentToken.Replace("'", "") });
                Scan();
                return true;
            }
            return false;
        }

        private bool OptionalSyntaxToken(List<ISyntaxToken> tokens)
        {
            if (IsMatch("["))
            {
                var optionalTokens = new List<ISyntaxToken>();

                if (SyntaxExpression(optionalTokens))
                {
                    if (!IsMatch("]"))
                    {
                        throw new Exception("Missing ]");
                    }

                    tokens.Add(new SubSyntaxToken() { Tokens = optionalTokens, IsOptional = true });
                    return true;
                }
            }

            return false;
        }

        private bool MultipleOptionSyntaxToken(List<ISyntaxToken> tokens)
        {
            if (IsMatch("("))
            {
                var optionalTokens = new List<ISyntaxToken>();
                var subExprTokens = new List<ISyntaxToken>();
                if (SyntaxExpression(subExprTokens))
                {
                    if (subExprTokens.Count > 1)
                    {
                        optionalTokens.Add(new SubSyntaxToken() { Tokens = subExprTokens });
                    }
                    else
                    {
                        //Avoid unnecessary nesting of SubSyntaxTokens
                        optionalTokens.Add(subExprTokens[0]);
                    }

                    while (true)
                    {
                        if (IsMatch("|"))
                        {
                            var nextSubExprTokens = new List<ISyntaxToken>();
                            if (SyntaxExpression(nextSubExprTokens))
                            {
                                if (nextSubExprTokens.Count > 1)
                                {
                                    optionalTokens.Add(new SubSyntaxToken() { Tokens = nextSubExprTokens });
                                }
                                else
                                {
                                    //Avoid unnecessary nesting of SubSyntaxTokens
                                    optionalTokens.Add(nextSubExprTokens[0]);
                                }
                            }
                            else
                            {
                                throw new Exception("Missing expression after |");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (!IsMatch(")"))
                    {
                        throw new Exception("Missing )");
                    }

                    TokenRepeatType repeatType = TokenRepeatType.Single;

                    if(IsMatch("*"))
                    {
                        repeatType = TokenRepeatType.ZeroOrMore;
                    }
                    else if(IsMatch("+"))
                    {
                        repeatType = TokenRepeatType.OneOrMore;
                    }

                    tokens.Add(new MultipleOptionSyntaxToken() { Tokens = optionalTokens, RepeatType = repeatType });
                    return true;
                }
            }
            return false;
        }
    }
}
