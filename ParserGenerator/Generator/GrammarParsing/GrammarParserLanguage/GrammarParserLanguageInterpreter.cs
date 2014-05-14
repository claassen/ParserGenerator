using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage
{
    public class GrammarParserLanguageInterpreter
    {
        private List<ILanguageToken> _langTokens;
        private GrammarExpression _grammarExpr;

        public GrammarExpression InterpretGrammarParserLanguageTokens(List<ILanguageToken> tokens)
        {
            _langTokens = tokens;

            GrammarExpression();

            return _grammarExpr;
        }

        private void GrammarExpression()
        {
            //If tokens[0] = LiteralTOken and value = REGEX: then regex expression
            //  name = tokens[1].Name
            //  if tokens[2] == '=' EXPRESSION_VALUE()
            //else name = tokens[0].Name
            // if tokens[1] == '=' EXPRESSION_VALUE()

            
        }
    }
}
