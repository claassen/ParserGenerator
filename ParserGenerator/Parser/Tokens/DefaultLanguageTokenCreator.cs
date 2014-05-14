using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public class DefaultLanguageTokenCreator : ILanguageTokenCreator
    {
        public virtual string RootExpressionName()
        {
            return "PROGRAM";
        }

        public virtual ILanguageToken Create(string literalToken)
        {
            return new DefaultLanguageToken() { Name = "Iteral Token", Value = literalToken };
        }

        public virtual ILanguageToken Create(string expressionName, string expressionValue)
        {
            return new DefaultLanguageToken() { Name = expressionName, Value = expressionValue };
        }

        public virtual ILanguageToken Create(string expressionName, List<ILanguageToken> tokens)
        {
            return new DefaultLanguageSubToken() { Name = expressionName, Tokens = tokens };
        }
    }
}
