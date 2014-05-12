using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Generator.SyntaxParsing
{
    public interface ISyntaxToken
    {
    }

    public class SubSyntaxToken : ISyntaxToken
    {
        public List<ISyntaxToken> Tokens;
        public bool IsOptional;

        public override string ToString()
        {
            return string.Join(" ", Tokens.Select(t => t.ToString()));
        }
    }

    public class ExpressionSyntaxToken : ISyntaxToken
    {
        public string ExpressionName;

        public override string ToString()
        {
            return ExpressionName;
        }
    }

    public class LiteralSyntaxToken : ISyntaxToken
    {
        public string Text;

        public override string ToString()
        {
            return Text;
        }
    }

    public class MultipleOptionSyntaxToken : ISyntaxToken
    {
        public List<ISyntaxToken> Tokens;
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

            return description;
        }
    }

    public enum TokenRepeatType
    {
        Single,
        ZeroOrMore,
        OneOrMore
    }
}
