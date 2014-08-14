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
        private GrammarParser _parser;

        public ParserGenerator()
        {
            _parser = new GrammarParser();
        }

        public ParserGenerator(List<GrammarExpression> grammar)
        {
            foreach (var expression in grammar)
            {
                _expressionTable.Add(expression.Name, expression);
            }
        }

        public void AddExpression(string syntaxExpression)
        {
            GrammarExpression expression = _parser.ParseGrammarExpression(syntaxExpression);

            if (expression.Tokens != null &&
                expression.Tokens.Count > 0 &&
                expression.Tokens.First() is ExpressionGrammarToken &&
                ((ExpressionGrammarToken)expression.Tokens.First()).ExpressionName == expression.Name)
            {
                throw new Exception("Left recursion detected");
            }

            _expressionTable.Add(expression.Name, expression);
        }

        public LanguageParser GetParser()
        {
            return GetParser(new DefaultLanguageTokenCreator());
        }

        public LanguageParser GetParser(ILanguageTokenCreator tokenCreator)
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
                        if (i == j) continue;

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

            return new LanguageParser(_expressionTable, tokenCreator);
        }
    }
}
