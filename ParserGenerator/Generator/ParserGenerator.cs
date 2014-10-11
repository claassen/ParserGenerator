using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator.Exceptions;
using ParserGen.Generator.GrammarParsing;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator
{
    public class ParserGenerator
    {
        private Dictionary<string, GrammarExpression> _expressionTable = new Dictionary<string, GrammarExpression>();
        private Dictionary<string, ILanguageToken> _userTokens;
        private GrammarParser _parser;

        public ParserGenerator(List<GrammarExpression> grammar)
        {
            foreach (var expression in grammar)
            {
                _expressionTable.Add(expression.Name, expression);
            }
        }

        public ParserGenerator(List<string> expressions)
        {
            Init();

            expressions.ForEach(e => AddExpression(e));
        }

        public ParserGenerator(List<ILanguageToken> expressions)
        {
            Init();

            _userTokens = new Dictionary<string, ILanguageToken>();

            expressions.ForEach(e => AddExpression(e));
        }

        private void Init()
        {
            _parser = new GrammarParser();
        }

        private void AddExpression(string syntaxExpression)
        {
            GrammarExpression expression = _parser.ParseGrammarExpression(syntaxExpression);

            _expressionTable.Add(expression.Name, expression);
        }

        private void AddExpression(ILanguageToken t)
        {
            var tokenAttr = t.GetType().GetCustomAttributes(typeof(TokenExpression), false).FirstOrDefault() as TokenExpression;
            
            if (tokenAttr == null)
            {
                throw new InvalidGrammarException("Missing [TokenExpression] class attribute.");
            }

            string name = tokenAttr.Name;
            string pattern = tokenAttr.Pattern;

            if (t is ILanguageTerminalToken && !name.StartsWith("REGEX:"))
            {
                name = "REGEX:" + name;
            }

            GrammarExpression expression = _parser.ParseGrammarExpression(name + " = " + pattern);

            _expressionTable.Add(expression.Name, expression);
            _userTokens.Add(expression.Name, t);
        }

        public LanguageParser GetParser(List<string> ignoreLiterals = null)
        {
            return GetParser(new DefaultLanguageTokenCreator(_userTokens), ignoreLiterals);
        }

        public LanguageParser GetParser(ILanguageTokenCreator tokenCreator, List<string> ignoreLiterals = null)
        {
            ValidateGrammar();

            return new LanguageParser(_expressionTable, tokenCreator, ignoreLiterals);
        }

        private string _currentLeftRecursionExprName;

        private void ValidateGrammar()
        {
            foreach (var expression in _expressionTable)
            {
                if (expression.Value.Tokens != null)
                {
                    _currentLeftRecursionExprName = expression.Value.Name;
                    _checkedExpressions = new HashSet<string>();
                    CheckLeftRecursion(expression.Value.Tokens.First());
                }
            }
        }

        private HashSet<string> _checkedExpressions;

        private void CheckLeftRecursion(IGrammarToken token)
        {
            if (token is GroupGrammarToken)
            {
                foreach (var t in ((GroupGrammarToken)token).Tokens)
                {
                    CheckLeftRecursion(t);
                }
            }
            else if (token is TokenListGrammarToken)
            {
                //if (((TokenListGrammarToken)token).Tokens.Count == 1)
                //{
                    CheckLeftRecursion(((TokenListGrammarToken)token).Tokens.First());
                //}
            }
            else
            {
                if (token is ExpressionGrammarToken)
                {
                    if (((ExpressionGrammarToken)token).ExpressionName == _currentLeftRecursionExprName)
                    {
                        throw new InvalidGrammarException("Left recursion detected in grammar expression: " + _currentLeftRecursionExprName);
                    }

                    if(!_checkedExpressions.Contains(((ExpressionGrammarToken)token).ExpressionName))
                    {
                        _checkedExpressions.Add(((ExpressionGrammarToken)token).ExpressionName);

                        var exp = _expressionTable[((ExpressionGrammarToken)token).ExpressionName];

                        if (exp.Tokens != null)
                        {
                            CheckLeftRecursion(exp.Tokens.First());
                        }
                    }
                }
            }
        }
    }
}
