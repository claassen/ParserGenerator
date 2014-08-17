using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public abstract class DefaultLanguageToken : ILanguageToken
    {
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }

    public class DefaultLanguageTerminalToken : DefaultLanguageToken
    {
        public string Value;
    }

    public class DefaultLanguageNonTerminalToken : DefaultLanguageToken
    {
        public List<ILanguageToken> Tokens { get; set; }
    }
}
