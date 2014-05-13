using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser
{
    public abstract class ILanguageTokenCreator
    {
        public abstract string RootExpressionName();
        public abstract ILanguageToken Create(string expressionName, string expressionValue);
    }
}
