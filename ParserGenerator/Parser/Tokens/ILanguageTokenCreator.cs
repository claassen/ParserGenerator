using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    public interface ILanguageTokenCreator
    {
        string RootExpressionName();
        ILanguageToken Create(string literalToken);
        ILanguageToken Create(string expressionName, string expressionValue);
        ILanguageToken Create(string expressionName, List<ILanguageToken> tokens);
    }
}
