using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UserLanguageToken : System.Attribute
    {
        public string Name;
        public string Pattern;

        public UserLanguageToken(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }
    }
}
