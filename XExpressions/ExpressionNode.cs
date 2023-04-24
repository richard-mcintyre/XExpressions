using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions
{
    public class ExpressionNode
    {
        #region Construction

        public ExpressionNode(Token token)
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
