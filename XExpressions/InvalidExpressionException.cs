using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions
{
    public class InvalidExpressionException : Exception
    {
        internal InvalidExpressionException(string msg)
            : base(msg)
        {
        }
    }
}
