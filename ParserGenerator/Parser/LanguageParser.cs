using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ParserGen.Generator.GrammarParsing;

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

        private void Scan()
        {
            string[] tokens = Regex.Split(_input, @"\s*('[^']+'|[()]|[[\]]|[|]|;|,)\s*|[\s[\]|\(\);,]");
            //string[] tokens = Regex.Split(_input, @"\s*('[^']+'|[()]|[[\]]|;|,)\s*|[\s[\]|\(\);,]");
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

        private void ParseSyntaxExpression(GrammarExpression expression, List<ILanguageToken> tokens)
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
                
                foreach (IGrammarToken exprToken in expression.Tokens)
                {
                    if (!IsMatch(exprToken))
                    {
                        throw new Exception("Syntax error. Expecting: " + exprToken.ToString());
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
                    throw new Exception("Syntax error. Expecting: " + t.ToString());
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
                throw new Exception("Syntax error. Expecting: " + exprToken.Text);
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

            if (!globalSuccess && (exprToken.RepeatType != TokenRepeatType.ZeroOrMore && exprToken.RepeatType != TokenRepeatType.Optional))
            {
                throw new Exception("Syntax error. Expecting: " + exprToken.ToString());
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
                    return _currentToken != null &&
                    Regex.IsMatch(_currentToken, ((RegexExpression)expression).Expression) &&
                    !_reservedWords.Contains(_currentToken);
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

            throw new Exception("Unknown SyntaxToken type");
        }
    }
}
