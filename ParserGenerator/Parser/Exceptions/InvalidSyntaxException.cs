using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserGen.Parser.Exceptions
{
    public class InvalidSyntaxException : Exception
    {
        public int Column;

        public InvalidSyntaxException(string message, int column, InvalidSyntaxException innerException = null)
            : base(message, innerException)
        {
            Column = column;
        }

        public override string ToString()
        {
            return Message + " @ Column: " + Column;
        }
    }
}
