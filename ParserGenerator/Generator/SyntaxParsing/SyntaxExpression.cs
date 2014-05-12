using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Generator.SyntaxParsing
{
    public class SyntaxExpression
    {
        public string Name;
        public List<ISyntaxToken> Tokens;
        
        public override string ToString()
        {
            return Name;
        }
    }

    public class RegexExpression : SyntaxExpression
    {
        public string Expression;
    }
}
