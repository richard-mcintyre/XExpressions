using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    /// <summary>
    /// Evaluates and expression
    /// </summary>
    public class Evaluator
    {
        #region TreeEvaluator

        /// <summary>
        /// Handles evaluating a tree representing an expression
        /// </summary>
        class TreeEvaluator
        {
            #region Construction

            public TreeEvaluator(XExpressionsSettings settings)
            {
                _settings = settings;
            }

            #endregion

            #region Fields

            private readonly XExpressionsSettings _settings;

            #endregion

            #region Methods

            /// <summary>
            /// Evaluates a tree node and its children
            /// </summary>
            /// <param name="node">Node to be evaluated</param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
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
                if (_settings.TryGetIdentifier(node.Token.Value, out IdentifierDef? identifier))
                {
                    Variant? identValue = identifier.fnGetValue.Invoke(node.Token.Value);
                    if (identValue == null)
                        throw new InvalidExpressionException($"Unknown identifier {node.Token.Value}");

                    return identValue.Value;
                }
                else
                    throw new InvalidExpressionException($"Unknown identifier {node.Token.Value}");
            }

            private Variant Handle_Function(ExpressionNode node)
            {
                if (_settings.TryGetFunction(node.Token.Value, out FunctionDef? function))
                {
                    Variant[] parameters = new Variant[function.ParameterCount];

                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = Evaluate(node.Children[i]);

                    return function.fnFuncImplementation(function.Name, parameters);
                }

                throw new InvalidExpressionException($"Unknown function {node.Token.Value}");
            }

            #endregion
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initalizes the evaluator with the specified expression
        /// </summary>
        /// <param name="expression">A string containing the expression to be evalauted</param>
        public Evaluator(string expression)
            : this(expression, XExpressionsSettings.Default)
        {
        }

        /// <summary>
        /// Initializes the evaluator with the specified expression using the specified settings
        /// </summary>
        /// <param name="expression">A string containing the expression to be evalauted</param>
        /// <param name="settings">Settings used for evaluating the expression</param>
        public Evaluator(string expression, XExpressionsSettings settings)
            : this(new Lexer(expression, settings), settings)
        {
        }

        /// <summary>
        /// Initializes the evaluator with a lexer that provides the tokens representing
        /// the expression.
        /// </summary>
        /// <param name="lexer">An instance of a lexer that provides the tokens representing the expression</param>
        public Evaluator(ILexer lexer)
            : this(lexer, XExpressionsSettings.Default)
        {
        }

        /// <summary>
        /// Initializes the evaluator with the specified lexer that provides the tokens representing
        /// the expression using the specified settings
        /// </summary>
        /// <param name="lexer">An instance of a lexer that provides the tokens representing the expression</param>
        /// <param name="settings">Settings used for evaluating the expression</param>
        public Evaluator(ILexer lexer, XExpressionsSettings settings)
        {
            _lexer = lexer;
            _settings = settings;
        }

        /// <summary>
        /// Initializes the evaluator with the tree that represents the expression
        /// </summary>
        /// <param name="node">The root node of the tree that represents the expression</param>
        public Evaluator(ExpressionNode node)
            : this(node, XExpressionsSettings.Default)
        {
        }

        /// <summary>
        /// Initializes the evaluator with the tree that represents the expression using the specified settings
        /// </summary>
        /// <param name="node">The root node of the tree that represents the expression</param>
        /// <param name="settings">Settings used for evaluating the expression</param>
        public Evaluator(ExpressionNode node, XExpressionsSettings settings)
        {
            _rootNode = node;
            _settings = settings;
        }

        #endregion

        #region Fields

        private readonly ILexer? _lexer;
        private readonly XExpressionsSettings _settings;

        private ExpressionNode? _rootNode;        

        #endregion

        #region Properties

        /// <summary>
        /// The root node of the tree that represents the expression
        /// </summary>
        public ExpressionNode RootNode => GetRootExpressionNode();

        #endregion

        #region Methods

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <returns>Result of the expression evaluation</returns>
        public Variant Evaluate()
        {
            ExpressionNode rootNode = GetRootExpressionNode();

            TreeEvaluator eval = new TreeEvaluator(_settings);
            return eval.Evaluate(rootNode);
        }

        private ExpressionNode GetRootExpressionNode()
        {
            // If we have not built the tree representing the expression yet
            if (_rootNode == null)
            {
                if (_lexer == null)
                    throw new Exception("A lexer is required");

                Parser tree = new Parser(_lexer, _settings);
                _rootNode = tree.Evaluate();
            }

            return _rootNode;
        }

        #endregion
    }

}
