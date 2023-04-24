using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

            private static readonly TaskFactory _taskFactory = new TaskFactory(
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

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
                // Running an async method from a sync method is not ideal - however, this implementation is used by MS here
                // https://github.com/aspnet/AspNetIdentity/blob/main/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs

                CultureInfo cultureUI = CultureInfo.CurrentUICulture;
                CultureInfo culture = CultureInfo.CurrentCulture;

                return _taskFactory.StartNew(() =>
                {
                    Thread.CurrentThread.CurrentUICulture = cultureUI;
                    Thread.CurrentThread.CurrentCulture = culture;

                    return EvaluateAsync(node, CancellationToken.None).AsTask();
                }).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Evaluates a tree node and its children
            /// </summary>
            /// <param name="node">Node to be evaluated</param>
            /// <param name="cancellation"></param>
            /// <returns></returns>
            /// <exception cref="Exception"></exception>
            public async ValueTask<Variant> EvaluateAsync(ExpressionNode node, CancellationToken cancellation)
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
                        return await Handle_IdentifierAsync(node, cancellation);

                    case TokenKind.AddOperator:
                        return await Handle_AddAsync(node, cancellation);

                    case TokenKind.SubtractOperator:
                        return await Handle_SubtractAsync(node, cancellation);

                    case TokenKind.MultiplyOperator:
                        return await Handle_MultiplyAsync(node, cancellation);

                    case TokenKind.DivideOperator:
                        return await Handle_DivideAsync(node, cancellation);

                    case TokenKind.Equals:
                        return await Handle_EqualsAsync(node, cancellation);

                    case TokenKind.Function:
                        return await Handle_FunctionAsync(node, cancellation);
                }

                throw new Exception($"Unknown token: {node.Token.Kind}");
            }

            private async ValueTask<Variant> Handle_AddAsync(ExpressionNode node, CancellationToken cancellation)
            {
                Variant left = await EvaluateAsync(node.Children[0], cancellation);
                Variant right = await EvaluateAsync(node.Children[1], cancellation);

                return left + right;
            }

            private async ValueTask<Variant> Handle_SubtractAsync(ExpressionNode node, CancellationToken cancellation)
            {
                Variant left = await EvaluateAsync(node.Children[0], cancellation);
                Variant right = await EvaluateAsync(node.Children[1], cancellation);

                return left - right;
            }

            private async ValueTask<Variant> Handle_MultiplyAsync(ExpressionNode node, CancellationToken cancellation)
            {
                Variant left = await EvaluateAsync(node.Children[0], cancellation);
                Variant right = await EvaluateAsync(node.Children[1], cancellation);

                return left * right;
            }

            private async ValueTask<Variant> Handle_DivideAsync(ExpressionNode node, CancellationToken cancellation)
            {
                Variant left = await EvaluateAsync(node.Children[0], cancellation);
                Variant right = await EvaluateAsync(node.Children[1], cancellation);

                return left / right;
            }

            private async ValueTask<Variant> Handle_EqualsAsync(ExpressionNode node, CancellationToken cancellation)
            {
                Variant left = await EvaluateAsync(node.Children[0], cancellation);
                Variant right = await EvaluateAsync(node.Children[1], cancellation);

                return left == right;
            }

            private async ValueTask<Variant> Handle_IdentifierAsync(ExpressionNode node, CancellationToken cancellation)
            {
                if (_settings.TryGetIdentifier(node.Token.Value, out IdentifierDef? identifier))
                {
                    Variant? identValue;

                    if (identifier.IsAsync)
                    {
                        identValue = await identifier.GetValueAsync(cancellation);
                    }
                    else
                    {
                        identValue = identifier.GetValue();
                    }

                    if (identValue != null)
                        return identValue.Value;
                }

                throw new InvalidExpressionException($"Unknown identifier {node.Token.Value}");
            }

            private async ValueTask<Variant> Handle_FunctionAsync(ExpressionNode node, CancellationToken cancellation)
            {
                if (_settings.TryGetFunction(node.Token.Value, out FunctionDef? function))
                {
                    Variant[] parameters = new Variant[function.ParameterCount];

                    for (int i = 0; i < parameters.Length; i++)
                        parameters[i] = await EvaluateAsync(node.Children[i], cancellation);

                    if (function.IsAsync)
                        return await function.InvokeAsync(parameters, cancellation);

                    return function.Invoke(parameters);
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

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns>Result of the expression evaluation</returns>
        public async ValueTask<Variant> EvaluateAsync(CancellationToken cancellation = default)
        {
            ExpressionNode rootNode = GetRootExpressionNode();

            TreeEvaluator eval = new TreeEvaluator(_settings);
            return await eval.EvaluateAsync(rootNode, cancellation);
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
