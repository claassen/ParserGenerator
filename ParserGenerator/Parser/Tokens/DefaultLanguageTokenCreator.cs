using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public class DefaultLanguageTokenCreator : ILanguageTokenCreator
    {
        public override string RootExpressionName()
        {
            return "PROGRAM";
        }

        public override ILanguageToken Create(string expressionName, string expressionValue)
        {
            return new ILanguageToken() { Name = expressionName, Value = expressionValue };
        }
    }
}
