using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace ParserGen.Generator.GrammarParsing.GrammarParserLanguage.Tokens
{
    internal class GPLExpressionToken : DefaultLanguageNonTerminalToken
    {
        public string ExpressionName;
    }

    internal class GPLIdentifierToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    internal class GPLRegexIdentififerToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    internal class GPLRegexExpressionToken : DefaultLanguageTerminalToken
    {
        public string Expression;
    }

    internal class GPLTokenToken : DefaultLanguageTerminalToken
    {
    }

    internal class GPLExprNameToken : DefaultLanguageTerminalToken
    {
        public string ExpressionName;
    }

    internal class GPLUserLiteralToken : DefaultLanguageTerminalToken
    {
        public string Text;
    }

    internal class GPLLiteralToken : DefaultLanguageTerminalToken
    {
        public string Text;
    }

    internal class GPLGroupToken : DefaultLanguageNonTerminalToken
    {
    }

    internal class GPLTokenListToken : DefaultLanguageNonTerminalToken
    {
    }
}
