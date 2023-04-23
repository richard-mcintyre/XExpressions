using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XExpressions
{
    /// <summary>
    /// Given an expression (from a lexer) builds a tree representing the expression
    /// </summary>
    public class Parser
    {
        #region Construction

        static Parser()
        {
            _expectedInitialTokens = new HashSet<TokenKind>();
            _expectedNextTokens = new Dictionary<TokenKind, HashSet<TokenKind>>();

            _expectedInitialTokens.Add(TokenKind.Number);
            _expectedInitialTokens.Add(TokenKind.Boolean);
            _expectedInitialTokens.Add(TokenKind.String);
            _expectedInitialTokens.Add(TokenKind.Identifier);
            _expectedInitialTokens.Add(TokenKind.OpenBracket);
            _expectedInitialTokens.Add(TokenKind.Function);

            HashSet<TokenKind> expectedTokenAfterValueOrIdent = new HashSet<TokenKind>()
            {
                TokenKind.AddOperator,
                TokenKind.SubtractOperator,
                TokenKind.MultiplyOperator,
                TokenKind.DivideOperator,
                TokenKind.Equals,
                TokenKind.CloseBracket,
                TokenKind.Comma,
                TokenKind.EOF
            };

            _expectedNextTokens.Add(TokenKind.Number, expectedTokenAfterValueOrIdent);
            _expectedNextTokens.Add(TokenKind.Boolean, expectedTokenAfterValueOrIdent);
            _expectedNextTokens.Add(TokenKind.String, expectedTokenAfterValueOrIdent);
            _expectedNextTokens.Add(TokenKind.Identifier, expectedTokenAfterValueOrIdent);
            _expectedNextTokens.Add(TokenKind.CloseBracket, expectedTokenAfterValueOrIdent);

            HashSet<TokenKind> expectedTokenAfterOperator = new HashSet<TokenKind>()
            {
                TokenKind.Number,
                TokenKind.Boolean,
                TokenKind.String,
                TokenKind.Identifier,
                TokenKind.OpenBracket,
                TokenKind.Function,
            };

            _expectedNextTokens.Add(TokenKind.AddOperator, expectedTokenAfterOperator);
            _expectedNextTokens.Add(TokenKind.SubtractOperator, expectedTokenAfterOperator);
            _expectedNextTokens.Add(TokenKind.MultiplyOperator, expectedTokenAfterOperator);
            _expectedNextTokens.Add(TokenKind.DivideOperator, expectedTokenAfterOperator);
            _expectedNextTokens.Add(TokenKind.Equals, expectedTokenAfterOperator);
            _expectedNextTokens.Add(TokenKind.Comma, expectedTokenAfterOperator);

            HashSet<TokenKind> expectedTokenAfterOpenBracket = new HashSet<TokenKind>()
            {
                TokenKind.Number,
                TokenKind.Boolean,
                TokenKind.String,
                TokenKind.Identifier,
                TokenKind.OpenBracket,
                TokenKind.Function,
                TokenKind.CloseBracket
            };

            _expectedNextTokens.Add(TokenKind.OpenBracket, expectedTokenAfterOpenBracket);

            _expectedNextTokens.Add(TokenKind.Function, new HashSet<TokenKind>()
            {
                TokenKind.OpenBracket
            });
        }

        public Parser(ILexer lex)
        {
            _lex = lex;
        }

        #endregion

        #region Fields

        private readonly ILexer _lex;
        private readonly static HashSet<TokenKind> _expectedInitialTokens;
        private readonly static Dictionary<TokenKind, HashSet<TokenKind>> _expectedNextTokens;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a tree for the expression
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidExpressionException"></exception>
        public ExpressionNode Evaluate()
        {
            Stack<ExpressionNode> nodes = new Stack<ExpressionNode>();

            foreach (Token token in GetOrderedTokens())
            {
                switch (token.Kind)
                {
                    case TokenKind.AddOperator:
                    case TokenKind.SubtractOperator:
                    case TokenKind.MultiplyOperator:
                    case TokenKind.DivideOperator:
                    case TokenKind.Equals:
                        HandleOperator(token, nodes);
                        break;

                    case TokenKind.Function:
                        HandleFunctionCall(token, nodes);
                        break;

                    default:
                        nodes.Push(new ExpressionNode(token));
                        break;
                }
            }

            if (nodes.Count != 1)
                throw new InvalidExpressionException("Invalid expression");

            ExpressionNode finalResult = nodes.Pop();

            return finalResult;
        }

        private static int GetPrecedence(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Equals:
                    return 1;

                case TokenKind.AddOperator:
                case TokenKind.SubtractOperator:
                    return 2;

                case TokenKind.MultiplyOperator:
                case TokenKind.DivideOperator:
                    return 3;
            }

            return 0;
        }

        private void ThrowUnexpectedToken(Token badToken, IEnumerable<TokenKind> expectedTokens)
        {
            if (expectedTokens.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (TokenKind token in expectedTokens)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    sb.Append(token.ToString());
                }

                if (badToken.Kind == TokenKind.EOF)
                    throw new InvalidExpressionException($"Expected tokens: {sb}");

                throw new InvalidExpressionException($"Unexpected token at position {badToken.Position}: '{badToken.Value}', expected {sb}");
            }

            // We didnt expect any tokens
            if (badToken.Kind == TokenKind.EOF)
                throw new InvalidExpressionException("Expected a token!");

            throw new InvalidExpressionException($"Unexpected token at position {badToken.Position}: '{badToken.Value}'");
        }

        /// <summary>
        /// Gets the tokens in the order that needed to evaluate them using the Shunting yard algorithm.
        /// 
        /// See https://en.wikipedia.org/wiki/Shunting_yard_algorithm
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidExpressionException"></exception>
        private IEnumerable<Token> GetOrderedTokens()
        {
            Stack<Token> operatorStack = new Stack<Token>();
            Token? lastToken = null;

            foreach (Token token in _lex)
            {
                HashSet<TokenKind> expectedTokens;
                if (lastToken == null)
                    expectedTokens = _expectedInitialTokens;
                else
                {
                    if (_expectedNextTokens.TryGetValue(lastToken.Kind, out HashSet<TokenKind>? tokens))
                        expectedTokens = tokens;
                    else
                        expectedTokens = new HashSet<TokenKind>();
                }

                if (expectedTokens.Contains(token.Kind) == false)
                    ThrowUnexpectedToken(token, expectedTokens);

                lastToken = token;

                if (token.Kind == TokenKind.EOF)
                    break;

                if (token.IsValueOrIdentifier)
                {
                    yield return token;
                }
                else if (token.Kind == TokenKind.Function)
                {
                    operatorStack.Push(token);
                }
                else if (token.IsOperatorOrEquals)
                {
                    while (operatorStack.Any() && operatorStack.Peek().Kind != TokenKind.OpenBracket &&
                           (GetPrecedence(operatorStack.Peek().Kind) >= GetPrecedence(token.Kind)))
                    {
                        yield return operatorStack.Pop();
                    }

                    operatorStack.Push(token);
                }
                else if (token.Kind == TokenKind.Comma)
                {
                    while (operatorStack.Any() &&
                           operatorStack.Peek().Kind != TokenKind.OpenBracket)
                    {
                        yield return operatorStack.Pop();
                    }
                }
                else if (token.Kind == TokenKind.OpenBracket)
                {
                    operatorStack.Push(token);
                }
                else if (token.Kind == TokenKind.CloseBracket)
                {
                    int functionArgCount = 0;

                    while (operatorStack.Any() &&
                           operatorStack.Peek().Kind != TokenKind.OpenBracket)
                    {
                        functionArgCount++;
                        yield return operatorStack.Pop();
                    }

                    if (operatorStack.Any() == false ||
                        operatorStack.Peek().Kind != TokenKind.OpenBracket)
                    {
                        throw new InvalidExpressionException("Invalid expression");
                    }

                    operatorStack.Pop();    // pop the open bracket

                    // if a function is at the top
                    if (operatorStack.Any() && operatorStack.Peek().Kind == TokenKind.Function)
                        yield return operatorStack.Pop();
                }
                else
                {
                    throw new InvalidExpressionException($"Unknown token: {token}");
                }
            }

            while (operatorStack.Any())
            {
                Token token = operatorStack.Pop();
                if (token.Kind == TokenKind.OpenBracket)
                    throw new InvalidExpressionException("Mismatched brackets");

                yield return token;
            }
        }

        private void HandleOperator(Token @operator, Stack<ExpressionNode> nodes)
        {
            if (nodes.Count < 2)
                throw new InvalidExpressionException("Incomplete expression");

            ExpressionNode rightNode = nodes.Pop();
            ExpressionNode leftNode = nodes.Pop();

            ExpressionNode opNode = new ExpressionNode(@operator);
            opNode.Children.Add(leftNode);
            opNode.Children.Add(rightNode);

            nodes.Push(opNode);
        }

        private void HandleFunctionCall(Token funcToken, Stack<ExpressionNode> nodes)
        {
            if (String.Equals(funcToken.Value, "min", StringComparison.InvariantCultureIgnoreCase))
            {
                if (nodes.Count < 2)
                    throw new InvalidExpressionException("Invalid expression");

                ExpressionNode node2 = nodes.Pop();
                ExpressionNode node1 = nodes.Pop();

                ExpressionNode funcNode = new ExpressionNode(funcToken);
                funcNode.Children.Add(node1);
                funcNode.Children.Add(node2);

                nodes.Push(funcNode);
            }
            else
                throw new InvalidExpressionException($"Unknown function: {funcToken}");
        }

        #endregion

    }
}
