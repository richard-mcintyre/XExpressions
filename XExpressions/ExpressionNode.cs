using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions
{
    /// <summary>
    /// Represents a node in a tree that represents an expression
    /// </summary>
    public class ExpressionNode
    {
        #region Construction

        internal ExpressionNode(Token token)
        {
            this.Token = token;
        }

        #endregion

        #region Properties

        public Token Token { get; }

        public List<ExpressionNode> Children { get; } = new List<ExpressionNode>();

        #endregion
    }
}
