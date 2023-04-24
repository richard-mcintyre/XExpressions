using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    public class Evaluator
    {
        #region TreeEvaluator

        class TreeEvaluator
        {
            #region Construction

            public TreeEvaluator(Func<string, Variant?>? fnGetIdentifierValue = null)
            {
                _fnGetIdentifierValue = fnGetIdentifierValue ?? ((ident) => null);
            }

            #endregion

            #region Fields

            private readonly Func<string, Variant?> _fnGetIdentifierValue;

            #endregion

            #region Methods

            public Variant Evaluate(ExpressionNode node)
            {
                switch (node.Token.Kind)
                {
                    case TokenKind.Number:
                        return Convert.ToDecimal(node.Token.Value);

                    case TokenKind.String:
                        return node.Token.Value;

                    case TokenKind.Boolean:
                        return Convert.ToBoolean(node.Token.Value);

                    case TokenKind.Identifier:
                        return Handle_Identifier(node);

                    case TokenKind.AddOperator:
                        return Handle_Add(node);

                    case TokenKind.SubtractOperator:
                        return Handle_Subtract(node);

                    case TokenKind.MultiplyOperator:
                        return Handle_Multiply(node);

                    case TokenKind.DivideOperator:
                        return Handle_Divide(node);

                    case TokenKind.Equals:
                        return Handle_Equals(node);

                    case TokenKind.Function:
                        return Handle_Function(node);
                }

                throw new Exception($"Unknown token: {node.Token.Kind}");
            }

            private Variant Handle_Add(ExpressionNode node)
            {
                Variant left = Evaluate(node.Children[0]);
                Variant right = Evaluate(node.Children[1]);

                return left + right;
            }

            private Variant Handle_Subtract(ExpressionNode node)
            {
                Variant left = Evaluate(node.Children[0]);
                Variant right = Evaluate(node.Children[1]);

                return left - right;
            }

            private Variant Handle_Multiply(ExpressionNode node)
            {
                Variant left = Evaluate(node.Children[0]);
                Variant right = Evaluate(node.Children[1]);

                return left * right;
            }

            private Variant Handle_Divide(ExpressionNode node)
            {
                Variant left = Evaluate(node.Children[0]);
                Variant right = Evaluate(node.Children[1]);

                return left / right;
            }

            private Variant Handle_Equals(ExpressionNode node)
            {
                Variant left = Evaluate(node.Children[0]);
                Variant right = Evaluate(node.Children[1]);

                return left == right;
            }

            private Variant Handle_Identifier(ExpressionNode node)
            {
                Variant? identValue = _fnGetIdentifierValue?.Invoke(node.Token.Value);
                if (identValue == null)
                    throw new InvalidExpressionException($"Unknown identifier {node.Token.Value}");

                return identValue.Value;
            }

            private Variant Handle_Function(ExpressionNode node)
            {
                if (String.Equals(node.Token.Value, "min", StringComparison.InvariantCultureIgnoreCase))
                {
                    Variant p1 = Evaluate(node.Children[0]);
                    Variant p2 = Evaluate(node.Children[1]);

                    return Math.Min((decimal)p1, (decimal)p2);
                }

                throw new InvalidExpressionException($"Unknown function {node.Token.Value}");
            }

            #endregion
        }

        #endregion

        #region Construction

        public Evaluator(string expression)
            : this(new Lexer(expression))
        {
        }

        public Evaluator(ILexer lexer)
        {
            _lexer = lexer;
        }

        public Evaluator(ExpressionNode node)
        {
            _rootNode = node;
        }

        #endregion

        #region Fields

        private readonly ILexer? _lexer;
        private ExpressionNode? _rootNode;

        #endregion

        #region Properties

        public ExpressionNode RootNode => GetRootExpressionNode();

        #endregion

        #region Methods

        public Variant Evaluate(Func<string, Variant?>? fnGetIdentifierValue = null)
        {
            ExpressionNode rootNode = GetRootExpressionNode();

            TreeEvaluator eval = new TreeEvaluator(fnGetIdentifierValue);
            return eval.Evaluate(rootNode);
        }

        private ExpressionNode GetRootExpressionNode()
        {
            if (_rootNode == null)
            {
                if (_lexer == null)
                    throw new Exception("A lexer is required");

                Parser tree = new Parser(_lexer);
                _rootNode = tree.Evaluate();
            }

            return _rootNode;
        }

        #endregion
    }

}
