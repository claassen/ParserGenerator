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

            _expressionTable.Add(expression.Name, expression);
        }

        public LanguageParser GetParser()
        {
            return GetParser(new DefaultLanguageTokenCreator());
        }

        public LanguageParser GetParser(ILanguageTokenCreator tokenCreator)
        {
            return new LanguageParser(_expressionTable, tokenCreator);
        }

        private IGrammarToken GetFirstSyntaxToken(GrammarExpression expression)
        {
            if (expression is RegexExpression)
            {
                return new ExpressionGrammarToken() { ExpressionName = expression.Name };
            }
            else
            {
                return expression.Tokens[0];
            }
        }
    }
}
