using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Tokens
{
    internal class DefaultLanguageTokenCreator : ILanguageTokenCreator
    {
        private Dictionary<string, ILanguageToken> _userTokens;

        public DefaultLanguageTokenCreator(Dictionary<string, ILanguageToken> userTokens)
        {
            _userTokens = userTokens;
        }

        public virtual ILanguageToken Create(string literalToken)
        {
            return new DefaultLanguageTerminalToken() { Name = "Literal Token", Value = literalToken };
        }

        public virtual ILanguageToken Create(string expressionName, string expressionValue)
        {
            if (_userTokens != null && _userTokens.ContainsKey(expressionName))
            {
                var userToken = (ILanguageTerminalToken)_userTokens[expressionName];
                return userToken.Create(expressionValue);
            }
            else
            {
                return new DefaultLanguageTerminalToken() { Name = expressionName, Value = expressionValue };
            }
        }

        public virtual ILanguageToken Create(string expressionName, List<ILanguageToken> tokens)
        {
            if (_userTokens != null && _userTokens.ContainsKey(expressionName))
            {
                var userToken = (ILanguageNonTerminalToken)_userTokens[expressionName];
                return userToken.Create(expressionName, tokens);
            }
            else
            {
                return new DefaultLanguageNonTerminalToken() { Name = expressionName, Tokens = tokens };
            }
        }
    }
}
