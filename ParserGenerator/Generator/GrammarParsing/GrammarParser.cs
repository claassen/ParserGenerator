using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserGen.Generator.GrammarParsing.GrammarParserLanguage;
using ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens;
using ParserGen.Parser;

namespace ParserGen.Generator.GrammarParsing
{
    public class GrammarParser
    {
        private string _input;
        private string _currentToken;

        //Documentation only
        private readonly string[] Expressions2 = new string[]
        {
            //@"GRAMMAR = (EXPRESSION)+",
            @"EXPRESSION = IDENTIFIER '=' TOKEN_LIST",
            @"REGEX:IDENTIFIER = (REGEX:)?[A-Z_]+",
            @"TOKEN_LIST = (TOKEN)+",
            @"TOKEN = (EXPR_NAME|LITERAL_TOKEN|GROUP_TOKEN)",
            @"REGEX:EXPR_NAME = [A-Z_]+",
            @"REGEX:LITERAL_TOKEN = '..*'",
            @"GROUP_TOKEN = '(' TOKEN_LIST ('|' TOKEN_LIST)* ')' "
        };

        private List<GrammarExpression> GetGrammarParserGrammarExpressions()
        {
            var expressions = new List<GrammarExpression>()
            {
                //new GrammarExpression()
                //{
                //    Name = "GRAMMAR",
                //    Tokens = new List<IGrammarToken>()
                //    {
                //        new MultipleOptionGrammarToken()
                //        {
                //            RepeatType = TokenRepeatType.OneOrMore,
                //            Tokens = new List<IGrammarToken>()
                //            {
                //                new SubGrammarToken()
                //                {
                //                    Tokens = new List<IGrammarToken>()
                //                    {
                //                        new ExpressionGrammarToken() { ExpressionName = "EXPRESSION" },
                //                        //new LiteralGrammarToken() { Text = "\n" }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //},

                new GrammarExpression()
                {
                    Name = "EXPRESSION",
                    Tokens = new List<IGrammarToken>()
                    {
                        new ExpressionGrammarToken() { ExpressionName = "IDENTIFIER" },
                        new LiteralGrammarToken() { Text = "=" },
                        new ExpressionGrammarToken() { ExpressionName = "TOKEN_LIST" }
                    }
                },

                new RegexExpression()
                {
                    Name = "IDENTIFIER",
                    Expression = "('REGEX:')? [A-Z_]+"
                },

                new GrammarExpression()
                {
                    Name = "TOKEN_LIST",
                    Tokens = new List<IGrammarToken>()
                    {
                        new GroupGrammarToken()
                        {
                            RepeatType = TokenRepeatType.OneOrMore,
                            Tokens = new List<IGrammarToken>()
                            {
                                new ExpressionGrammarToken() { ExpressionName = "TOKEN" }
                            }
                        }
                    }
                },

                new GrammarExpression()
                {
                    Name = "TOKEN",
                    Tokens = new List<IGrammarToken>()
                    {
                        new GroupGrammarToken()
                        {
                            RepeatType = TokenRepeatType.Single,
                            Tokens = new List<IGrammarToken>()
                            {
                                new ExpressionGrammarToken() { ExpressionName = "EXPR_NAME" },
                                new ExpressionGrammarToken() { ExpressionName = "LITERAL_TOKEN" },
                                new ExpressionGrammarToken() { ExpressionName = "GROUP_TOKEN" }
                            }
                        }
                    }
                },

                new RegexExpression()
                {
                    Name = "EXPR_NAME",
                    Expression = "[A-Z_]+"
                },

                new RegexExpression()
                {
                    Name = "LITERAL_TOKEN",
                    Expression = "'..*'"
                },

                new GrammarExpression()
                {
                    Name = "GROUP_TOKEN",
                    Tokens = new List<IGrammarToken>()
                    {
                        new LiteralGrammarToken() { Text = "(" },
                        new ExpressionGrammarToken() { ExpressionName = "TOKEN_LIST" }, 
                        new GroupGrammarToken()
                        {
                            RepeatType = TokenRepeatType.ZeroOrMore,
                            Tokens = new List<IGrammarToken>()
                            {
                                new TokenListGrammarToken()
                                {
                                    Tokens = new List<IGrammarToken>()
                                    {
                                        new LiteralGrammarToken() { Text = "|" },
                                        new ExpressionGrammarToken() { ExpressionName = "TOKEN_LIST" }, 
                                    }
                                }
                            }
                        },
                        new LiteralGrammarToken() { Text = ")" },
                        new GroupGrammarToken()
                        {
                            RepeatType = TokenRepeatType.Optional,
                            Tokens = new List<IGrammarToken>()
                            {
                                new LiteralGrammarToken() { Text = "*" },
                                new LiteralGrammarToken() { Text = "+" },
                                new LiteralGrammarToken() { Text = "?" }
                            }
                        }
                    }
                }
            };

            return expressions;
        }

        private LanguageParser _parser;
        private GrammarParserLanguageInterpreter _interpreter;

        public GrammarParser()
        {
            var generator = new ParserGenerator(GetGrammarParserGrammarExpressions());

            _parser = generator.GetParser(new GrammarParserTokenCreator());
            _interpreter = new GrammarParserLanguageInterpreter();
        }

        public void Test(string input)
        {
            var tokens = _parser.Parse(input);

            GrammarExpression expression = _interpreter.InterpretGrammarParserLanguageTokens(tokens);
        }

        public GrammarExpression ParseSyntaxExpression(string input)
        {
            _input = input;
            _currentToken = null;

            var tokens = new List<IGrammarToken>();

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

            var e = new GrammarExpression() { Name = expressionName };

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

        private bool SyntaxExpression(List<IGrammarToken> tokens)
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
                //else if (OptionalSyntaxToken(tokens))
                //{
                //    success = true;
                //}
                else if (GroupSyntaxToken(tokens))
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

        private bool ExpressionSyntaxToken(List<IGrammarToken> tokens)
        {
            if (_currentToken != null && Regex.IsMatch(_currentToken, @"^[A-Z_]+$"))
            {
                tokens.Add(new ExpressionGrammarToken() { ExpressionName = _currentToken });
                Scan();
                return true;
            }
            return false;
        }

        private bool LiteralSyntaxToken(List<IGrammarToken> tokens)
        {
            if (_currentToken != null && Regex.IsMatch(_currentToken, @"'..*'"))
            {
                tokens.Add(new LiteralGrammarToken() { Text = _currentToken.Replace("'", "") });
                Scan();
                return true;
            }
            return false;
        }

        //private bool OptionalSyntaxToken(List<IGrammarToken> tokens)
        //{
        //    if (IsMatch("["))
        //    {
        //        var optionalTokens = new List<IGrammarToken>();

        //        if (SyntaxExpression(optionalTokens))
        //        {
        //            if (!IsMatch("]"))
        //            {
        //                throw new Exception("Missing ]");
        //            }

        //            tokens.Add(new TokenListToken() { Tokens = optionalTokens, IsOptional = true });
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        private bool GroupSyntaxToken(List<IGrammarToken> tokens)
        {
            if (IsMatch("("))
            {
                var optionalTokens = new List<IGrammarToken>();
                var subExprTokens = new List<IGrammarToken>();
                if (SyntaxExpression(subExprTokens))
                {
                    if (subExprTokens.Count > 1)
                    {
                        optionalTokens.Add(new TokenListGrammarToken() { Tokens = subExprTokens });
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
                            var nextSubExprTokens = new List<IGrammarToken>();
                            if (SyntaxExpression(nextSubExprTokens))
                            {
                                if (nextSubExprTokens.Count > 1)
                                {
                                    optionalTokens.Add(new TokenListGrammarToken() { Tokens = nextSubExprTokens });
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

                    tokens.Add(new GroupGrammarToken() { Tokens = optionalTokens, RepeatType = repeatType });
                    return true;
                }
            }
            return false;
        }
    }
}
