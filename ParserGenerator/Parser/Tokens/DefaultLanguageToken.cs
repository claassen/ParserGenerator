using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public class DefaultLanguageToken : ILanguageToken
    {
        public string Name;
        public string Value;

        public override string ToString()
        {
            return Name;
        }
    }

    public class DefaultLanguageSubToken : ILanguageSubToken
    {
        public string Name;
    }
}
