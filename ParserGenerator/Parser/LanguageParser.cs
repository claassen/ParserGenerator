using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserGen.Generator.GrammarParsing;
using ParserGen.Parser.Exceptions;
using ParserGen.Parser.Tokens;

namespace ParserGen.Parser
{
    public class LanguageParser
    {
        private Dictionary<string, GrammarExpression> _expressionTable;
        private ILanguageTokenCreator _tokenCreator;
        private HashSet<string> _reservedWords;
        private HashSet<string> _ignoreLiterals;
        
        private string _input;
        private string _currentToken;

        private InvalidSyntaxException lastException;

        public LanguageParser(Dictionary<string, GrammarExpression> expressionTable, ILanguageTokenCreator tokenCreator, List<string> ignoreLiterals = null)
        {
            _expressionTable = expressionTable;
            _tokenCreator = tokenCreator;

            //Uses all literal tokens present in the given syntax as reserved keywords so that user supplied
            //regex expressions won't match language keywords or special purpose characters
            _reservedWords = new HashSet<string>();
            foreach (var expression in _expressionTable.Select(e => e.Value))
            {
                if (expression.Tokens != null)
                {
                    foreach (var t in expression.Tokens)
                    {
                        FindReservedKeywords(t);
                    }
                }
            }

            if (ignoreLiterals == null) ignoreLiterals = new List<string>();

            _ignoreLiterals = new HashSet<string>(ignoreLiterals.Union(new List<string>() { "(", ")", ",", ";" }));
        }

        private void FindReservedKeywords(IGrammarToken token)
        {
            if (token is LiteralGrammarToken)
            {
                _reservedWords.Add(((LiteralGrammarToken)token).Text);
            }
            else if (token is TokenListGrammarToken)
            {
                if (((TokenListGrammarToken)token).Tokens != null)
                {
                    foreach (var t in ((TokenListGrammarToken)token).Tokens)
                    {
                        FindReservedKeywords(t);
                    }
                }
            }
            else if (token is GroupGrammarToken)
            {
                if (((GroupGrammarToken)token).Tokens != null)
                {
                    foreach (var t in ((GroupGrammarToken)token).Tokens)
                    {
                        FindReservedKeywords(t);
                    }
                }
            }
        }

        private InvalidSyntaxException GetRootCauseException(InvalidSyntaxException ex)
        {
            InvalidSyntaxException rootCause = ex;

            while (rootCause.InnerException != null)
            {
                rootCause = (InvalidSyntaxException)rootCause.InnerException;
            }

            return rootCause;
        }

        private void Scan()
        {
            string[] delims = new string[]
            {
                "(", ")", "[", "]", "{", "}", "||", "|", "&&", "&", ";", ".", "*", "->", "<-", "-", " ", ",", "==", "!=", "<=", ">=", "<", ">", "="
            };

            string splitRegex = String.Format(
                @"\s*('[^']+'|""[^""]*""|{0})\s*|[{1}]",
                String.Join("|", delims.Select(d => Regex.Escape(d))),
                String.Join("", delims.Select(d => Regex.Escape(d)))
            );

            string[] tokens = Regex.Split(_input, splitRegex);

            _currentToken = tokens.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            if (!string.IsNullOrEmpty(_currentToken))
            {
                _input = _input.TrimStart().Substring(_currentToken.Length);
            }
        }

        private int GetCurrentSourceColumn()
        {
            if (originalSrc != null && _input != null && _currentToken != null)
            {
                return originalSrc.Length - _input.Length - _currentToken.Length;
            }
            else
            {
                return -1;
            }
        }

        private string originalSrc;

        public List<ILanguageToken> Parse(string source, string rootExpressionName = "PROGRAM")
        {
            originalSrc = source;

            _input = source.Replace("\n", "").Replace("\r", "");

            List<ILanguageToken> tokens = new List<ILanguageToken>();

            Scan();

            try
            {
                ParseSyntaxExpression(_expressionTable[rootExpressionName], tokens);
            }
            catch (InvalidSyntaxException ex)
            {
                throw GetRootCauseException(ex);
            }

            if (_currentToken != null)
            {
                if (lastException != null)
                {
                    throw GetRootCauseException(lastException);
                }
                else
                {
                    throw new InvalidSyntaxException("Syntax error. Unexpected token: " + _currentToken, GetCurrentSourceColumn());
                }
            }

            return tokens;
        }

        private void ParseSyntaxExpression(GrammarExpression expression, List<ILanguageToken> tokens)
        {
            if (expression is RegexExpression)
            {
                if (_currentToken != null && Regex.IsMatch(_currentToken, ((RegexExpression)expression).Expression))
                {
                    if (_reservedWords.Contains(_currentToken))
                    {
                        throw new InvalidSyntaxException("Use of reserved keyword: " + _currentToken, GetCurrentSourceColumn());
                    }

                    tokens.Add(_tokenCreator.Create(expression.Name, _currentToken));
                    Scan();
                }
                else
                {
                    lastException = new InvalidSyntaxException("Syntax error: " + _currentToken + ", Expecting: " + ((RegexExpression)expression).Name, GetCurrentSourceColumn(), lastException);
                    throw lastException;
                }
            }
            else
            {
                List<ILanguageToken> subTokens = new List<ILanguageToken>();
                
                foreach (IGrammarToken exprToken in expression.Tokens)
                {
                    if (!IsMatch(exprToken))
                    {
                        lastException = new InvalidSyntaxException("Syntax error: " + _currentToken + ", Expecting: " + exprToken.ToString(), GetCurrentSourceColumn(), lastException);
                        throw lastException;
                    }

                    ParseSyntaxToken(exprToken, subTokens);
                }

                tokens.Add(_tokenCreator.Create(expression.Name, subTokens));
            }
        }

        private void ParseSyntaxToken(IGrammarToken exprToken, List<ILanguageToken> tokens)
        {
            if (exprToken is TokenListGrammarToken)
            {
                ParseTokenListSyntaxToken((TokenListGrammarToken)exprToken, tokens);
            }
            else if (exprToken is ExpressionGrammarToken)
            {
                ParseExpressionSyntaxToken((ExpressionGrammarToken)exprToken, tokens);
            }
            else if (exprToken is LiteralGrammarToken)
            {
                ParseLiteralSyntaxToken((LiteralGrammarToken)exprToken, tokens);
            }
            else if (exprToken is GroupGrammarToken)
            {
                ParseGroupSyntaxToken((GroupGrammarToken)exprToken, tokens);
            }
        }

        private void ParseTokenListSyntaxToken(TokenListGrammarToken exprToken, List<ILanguageToken> tokens)
        {
            foreach (IGrammarToken t in exprToken.Tokens)
            {
                if (!IsMatch(t))
                {
                    lastException = new InvalidSyntaxException("Syntax error: " + _currentToken + ", Expecting: " + t.ToString(), GetCurrentSourceColumn(), lastException);
                    throw lastException;
                }

                ParseSyntaxToken(t, tokens);
            }
        }

        private void ParseExpressionSyntaxToken(ExpressionGrammarToken exprToken, List<ILanguageToken> tokens)
        {
            GrammarExpression expression = _expressionTable[exprToken.ExpressionName];

            ParseSyntaxExpression(expression, tokens);
        }

        private void ParseLiteralSyntaxToken(LiteralGrammarToken exprToken, List<ILanguageToken> tokens)
        {
            if (!IsMatch(exprToken))
            {
                lastException = new InvalidSyntaxException("Syntax error: " + _currentToken + ", Expecting: " + exprToken.Text, GetCurrentSourceColumn(), lastException);
                throw lastException;
            }

            if (!_ignoreLiterals.Contains(_currentToken))
            {
                tokens.Add(_tokenCreator.Create(_currentToken));
            }
            
            Scan();
        }

        private void ParseGroupSyntaxToken(GroupGrammarToken exprToken, List<ILanguageToken> tokens)
        {
            bool globalSuccess = false;

            string SAVED_INPUT = _input;
            string SAVED_TOKEN = _currentToken;

            InvalidSyntaxException lastInnerException = null;

            while (true)
            {
                bool success = false;
                
                foreach (IGrammarToken t in exprToken.Tokens)
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
                            SAVED_INPUT = _input;
                            SAVED_TOKEN = _currentToken;

                            break;
                        }
                        catch(InvalidSyntaxException ex)
                        {
                            //Backtrack
                            _input = SAVED_INPUT;
                            _currentToken = SAVED_TOKEN;

                            lastInnerException = ex;
                        }
                    }
                }

                if (!success || (exprToken.RepeatType != TokenRepeatType.ZeroOrMore && exprToken.RepeatType != TokenRepeatType.OneOrMore))
                {
                    break;
                }
            }

            if (!globalSuccess && (exprToken.RepeatType != TokenRepeatType.ZeroOrMore && exprToken.RepeatType != TokenRepeatType.Optional))
            {
                lastException = new InvalidSyntaxException("Syntax error: " + _currentToken + ", Expecting: " + exprToken.ToString(), GetCurrentSourceColumn(), lastInnerException);
                throw lastException;
            }
        }

        private bool IsMatch(IGrammarToken token)
        {
            if (token is TokenListGrammarToken)
            {
                return IsMatch(((TokenListGrammarToken)token).Tokens[0]);
            }
            else if (token is ExpressionGrammarToken)
            {
                GrammarExpression expression = _expressionTable[((ExpressionGrammarToken)token).ExpressionName];

                if (expression is RegexExpression)
                {
                    return _currentToken != null && Regex.IsMatch(_currentToken, ((RegexExpression)expression).Expression);
                }
                else
                {
                    return IsMatch(expression.Tokens[0]);
                }
            }
            else if(token is LiteralGrammarToken)
            {
                return _currentToken == ((LiteralGrammarToken)token).Text;
            }
            else if(token is GroupGrammarToken)
            {
                GroupGrammarToken groupToken = (GroupGrammarToken)token;
                bool match = false;

                foreach (IGrammarToken t in groupToken.Tokens)
                {
                    if (IsMatch(t))
                    {
                        match = true;
                        break;
                    }
                }

                return match || 
                       groupToken.RepeatType == TokenRepeatType.Optional || 
                       groupToken.RepeatType == TokenRepeatType.ZeroOrMore;
            }

            throw new InvalidSyntaxException("Unknown SyntaxToken type", GetCurrentSourceColumn());
        }
    }
}
