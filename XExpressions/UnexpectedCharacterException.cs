using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions
{
    public class UnexpectedCharacterException : Exception
    {
        #region Construction

        public UnexpectedCharacterException(string expression, int position)
            : base($"Unexpected character: {expression[position]}")
        {
            this.Expression = expression;
            this.Position = position;
        }

        #endregion

        #region Properties

        public string Expression { get; }

        public int Position { get; }

        public char Character => this.Expression[this.Position];

        #endregion
    }
}
