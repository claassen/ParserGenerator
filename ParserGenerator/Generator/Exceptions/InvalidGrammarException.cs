using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Generator.Exceptions
{
    public class InvalidGrammarException : Exception
    {
        public InvalidGrammarException(string message)
            : base(message)
        {
        }
    }
}
