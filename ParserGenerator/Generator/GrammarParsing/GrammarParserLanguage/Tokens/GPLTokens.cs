using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens
{
    public class GPLExpressionToken : DefaultLanguageNonTerminalToken
    {
        public string ExpressionName;
    }

    public class GPLIdentifierToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    public class GPLRegexIdentififerToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    public class GPLRegexExpressionToken : DefaultLanguageTerminalToken
    {
        public string Expression;
    }

    public class GPLTokenToken : DefaultLanguageTerminalToken
    {
    }

    public class GPLExprNameToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    public class GPLUserLiteralToken : DefaultLanguageTerminalToken
    {
        public string Text;
    }

    public class GPLLiteralToken : DefaultLanguageTerminalToken
    {
        public string Text;
    }

    public class GPLGroupToken : DefaultLanguageNonTerminalToken
    {
    }

    public class GPLTokenListToken : DefaultLanguageNonTerminalToken
    {
    }
}
