using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser
{
    public abstract class ILanguageToken
    {
    }

    public abstract class ILanguageSubToken : ILanguageToken
    {
        public List<ILanguageToken> Tokens;
    }
}
