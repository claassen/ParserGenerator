using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Generator.GrammarParsing
{
    public interface IGrammarToken
    {
    }

    public class TokenListGrammarToken : IGrammarToken
    {
        public List<IGrammarToken> Tokens;
        
        public override string ToString()
        {
            return string.Join(" ", Tokens.Select(t => t.ToString()));
        }
    }

    public class ExpressionGrammarToken : IGrammarToken
    {
        public string ExpressionName;

        public override string ToString()
        {
            return ExpressionName;
        }
    }

    public class LiteralGrammarToken : IGrammarToken
    {
        public string Text;

        public override string ToString()
        {
            return Text;
        }
    }

    public class GroupGrammarToken : IGrammarToken
    {
        public List<IGrammarToken> Tokens;
        public TokenRepeatType RepeatType = TokenRepeatType.Single;

        public override string ToString()
        {
            string description = "(" + string.Join("|", Tokens.Select(t => t.ToString())) + ")";
            
            if(RepeatType == TokenRepeatType.OneOrMore)
            {
                description += "+";
            }
            else if(RepeatType == TokenRepeatType.ZeroOrMore)
            {
                description += "*";
            }
            else if (RepeatType == TokenRepeatType.Optional)
            {
                description += "?";
            }

            return description;
        }
    }

    public enum TokenRepeatType
    {
        Single,
        Optional,
        ZeroOrMore,
        OneOrMore
    }
}
