using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public interface ILanguageToken
    {
    }

    public abstract class ILanguageTerminalToken : ILanguageToken
    {
        public abstract ILanguageToken Create(string expressionValue);
    }

    public abstract class ILanguageNonTerminalToken : ILanguageToken
    {
        public List<ILanguageToken> Tokens { get; set; }

        public abstract ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens);
    }
}
