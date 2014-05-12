using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parse
{
    public class ILanguageToken
    {
        public string Name;
        public string Value;

        public override string ToString()
        {
            return Name;
        }
    }

    public class ILanguageSubToken : ILanguageToken
    {
        public List<ILanguageToken> Tokens;
    }
}
