using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens
{
    //public class GrammarToken : ILanguageSubToken
    //{
    //}

    public class ExpressionToken : ILanguageSubToken
    {
        public string ExpressionName;
    }

    public class IdentifierToken : ILanguageToken
    {
        public string ExpressionName;
    }

    public class TokenToken : ILanguageToken
    {
    }

    public class ExprNameToken : ILanguageToken
    {
        public string ExpressionName;
    }

    public class LiteralToken : ILanguageToken
    {
        public string Text;
    }

    public class GroupToken : ILanguageSubToken
    {
    }

    public class TokenListToken : ILanguageSubToken
    {
    }
}
