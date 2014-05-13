using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Generator.GrammarParsing
{
    public class GrammarExpression
    {
        public string Name;
        public List<IGrammarToken> Tokens;
        
        public override string ToString()
        {
            return Name;
        }
    }

    public class RegexExpression : GrammarExpression
    {
        public string Expression;
    }
}
