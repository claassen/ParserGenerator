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
        //Documentation only
        private readonly string[] GPLExpressions = new string[]
        {
            //@"GRAMMAR = (EXPRESSION)+",
            @"EXPRESSION = (IDENTIFIER '=' TOKEN_LIST|REGEXIDENTIFIER '=' REGEX_EXPRESSION)",
            @"REGEX:IDENTIFIER = [A-Z_]+",
            @"REGEX:REGEXIDENTIFIER = REGEX:[A-Z_]+",
            @"TOKEN_LIST = (TOKEN)+",
            @"TOKEN = (EXPR_NAME|LITERAL_TOKEN|GROUP_TOKEN)",
            @"REGEX:EXPR_NAME = [A-Z_]+",
            @"REGEX:LITERAL_TOKEN = '..*'",
            @"REGEX:REGEX_EXPRESSION = '.+'",
            @"GROUP_TOKEN = '(' TOKEN_LIST ('|' TOKEN_LIST)* ')'"
        };

        private List<GrammarExpression> GetGPLGrammarExpressions()
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
                        new GroupGrammarToken()
                        {
                            RepeatType = TokenRepeatType.Single,
                            Tokens = new List<IGrammarToken>()
                            {
                                new TokenListGrammarToken()
                                {
                                    Tokens = new List<IGrammarToken>()
                                    {
                                        new ExpressionGrammarToken() { ExpressionName = "IDENTIFIER" },
                                        new LiteralGrammarToken() { Text = "=" },
                                        new ExpressionGrammarToken() { ExpressionName = "TOKEN_LIST" }
                                    }
                                },
                                new TokenListGrammarToken()
                                {
                                    Tokens = new List<IGrammarToken>()
                                    {
                                        new ExpressionGrammarToken() { ExpressionName = "REGEX_IDENTIFIER" },
                                        new LiteralGrammarToken() { Text = "=" },
                                        new ExpressionGrammarToken() { ExpressionName = "REGEX_EXPRESSION" }
                                    }
                                }
                            }
                        }

                        
                    }
                },

                new RegexExpression()
                {
                    Name = "IDENTIFIER",
                    Expression = @"^[A-Z_]+$"
                },

                new RegexExpression()
                {
                    Name = "REGEX_IDENTIFIER",
                    Expression = @"^REGEX\:[A-Z_]+$"
                },

                new RegexExpression()
                {
                    Name = "REGEX_EXPRESSION",
                    Expression = @"^.+$"
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
                    Expression = "^[A-Z_]+$"
                },

                new RegexExpression()
                {
                    Name = "LITERAL_TOKEN",
                    Expression = "^'..*'$"
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
        private GPLInterpreter _interpreter;

        public GrammarParser()
        {
            var generator = new ParserGenerator(GetGPLGrammarExpressions());

            _parser = generator.GetParser(new GPLTokenCreator());
            _interpreter = new GPLInterpreter();
        }

        public GrammarExpression ParseGrammarExpression(string input)
        {
            var tokens_ = _parser.Parse(input);

            return _interpreter.InterpretGPLTokens(tokens_);
        }
    }
}
