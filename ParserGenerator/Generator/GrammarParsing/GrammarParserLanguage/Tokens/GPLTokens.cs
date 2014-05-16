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

    public class GPLExpressionToken : ILanguageSubToken
    {
        public string ExpressionName;
    }

    public class GPLIdentifierToken : ILanguageToken
    {
        public string ExpressionName;
    }

    public class GPLRegexIdentififerToken : ILanguageToken
    {
        public string ExpressionName;
    }

    public class GPLRegexExpressionToken : ILanguageToken
    {
        public string Expression;
    }

    public class GPLTokenToken : ILanguageToken
    {
    }

    public class GPLExprNameToken : ILanguageToken
    {
        public string ExpressionName;
    }

    public class GPLUserLiteralToken : ILanguageToken
    {
        public string Text;
    }

    public class GPLLiteralToken : ILanguageToken
    {
        public string Text;
    }

    public class GPLGroupToken : ILanguageSubToken
    {
    }

    public class GPLTokenListToken : ILanguageSubToken
    {
    }
}
