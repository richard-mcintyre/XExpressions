using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions
{
    public enum TokenKind
    {
        Number,
        String,
        Boolean,
        Identifier,
        AddOperator,
        SubtractOperator,
        MultiplyOperator,
        DivideOperator,
        OpenBracket,
        CloseBracket,
        Equals,
        Function,
        Comma,
        EOF,
    }
}
