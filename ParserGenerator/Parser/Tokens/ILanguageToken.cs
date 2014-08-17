using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser
{
    public interface ILanguageToken
    {
    }

    

    public abstract class IUserLanguageToken : ILanguageToken
    {
    }

    public abstract class IUserLanguageTerminalToken : IUserLanguageToken
    {
        public abstract IUserLanguageToken Create(string expressionValue);
    }

    public abstract class IUserLanguageNonTerminalToken : IUserLanguageToken
    {
        public List<ILanguageToken> Tokens { get; set; }

        public abstract IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens);
    }
}
