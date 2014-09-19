using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Exceptions
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException(string message, int column, InvalidSyntaxException innerException = null)
            : base(message + " @ Column: " + column, innerException)
        {
        }
    }
}
