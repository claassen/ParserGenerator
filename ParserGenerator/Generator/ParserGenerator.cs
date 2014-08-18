using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator.GrammarParsing;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator
{
    public class ParserGenerator
    {
        private Dictionary<string, GrammarExpression> _expressionTable = new Dictionary<string, GrammarExpression>();
        private Dictionary<string, IUserLanguageToken> _userTokens;
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

        public ParserGenerator(List<IUserLanguageToken> expressions)
        {
            Init();

            _userTokens = new Dictionary<string, IUserLanguageToken>();

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

        private void AddExpression(IUserLanguageToken t)
        {
            var tokenAttr = t.GetType().GetCustomAttributes(typeof(UserLanguageToken), false).FirstOrDefault() as UserLanguageToken;
            
            if (tokenAttr == null)
            {
                throw new Exception("Missing [UserLanguageToken] class attribute.");
            }

            string name = tokenAttr.Name;
            string pattern = tokenAttr.Pattern;

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

        private void ValidateGrammar()
        {
            for (int i = 0; i < _expressionTable.Count; i++)
            {
                var exp1 = _expressionTable.ElementAt(i);

                if (exp1.Value.Tokens != null)
                {
                    var firstToken1 = exp1.Value.Tokens.First();

                    while (firstToken1 is GroupGrammarToken || firstToken1 is TokenListGrammarToken)
                    {
                        if (firstToken1 is GroupGrammarToken)
                        {
                            firstToken1 = ((GroupGrammarToken)firstToken1).Tokens.First();
                        }
                        else
                        {
                            firstToken1 = ((TokenListGrammarToken)firstToken1).Tokens.First();
                        }
                    }

                    for (int j = 0; j < _expressionTable.Count; j++)
                    {
                        if (i == j)
                        {
                            if (firstToken1 is ExpressionGrammarToken)
                            {
                                if (((ExpressionGrammarToken)firstToken1).ExpressionName == _expressionTable.ElementAt(j).Value.Name)
                                {
                                    throw new Exception("Left recursion detected");
                                }
                            }

                            continue;
                        } 

                        var exp2 = _expressionTable.ElementAt(j);

                        if (exp2.Value.Tokens != null)
                        {
                            var firstToken2 = exp2.Value.Tokens.First();

                            while (firstToken2 is GroupGrammarToken || firstToken2 is TokenListGrammarToken)
                            {
                                if (firstToken2 is GroupGrammarToken)
                                {
                                    firstToken2 = ((GroupGrammarToken)firstToken2).Tokens.First();
                                }
                                else
                                {
                                    firstToken2 = ((TokenListGrammarToken)firstToken2).Tokens.First();
                                }
                            }

                            if (firstToken1 is ExpressionGrammarToken && firstToken2 is ExpressionGrammarToken)
                            {
                                if (((ExpressionGrammarToken)firstToken1).ExpressionName == exp2.Value.Name &&
                                    ((ExpressionGrammarToken)firstToken2).ExpressionName == exp1.Value.Name)
                                {
                                    throw new Exception("Mutual recursion");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
