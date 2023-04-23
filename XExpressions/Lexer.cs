using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XExpressions
{
    public class Lexer : ILexer
    {
        #region TokenEnumerator

        class TokenEnumerator : IEnumerator<Token>
        {
            #region Construction

            public TokenEnumerator(Lexer lex)
            {
                _lex = lex;
            }

            #endregion

            #region Fields

            private readonly Lexer _lex;
            private int _position;
            private Token? _currentToken;
            private bool _isDisposed;

            #endregion

            #region Properties

            public Token Current => GetCurrent();

            object IEnumerator.Current => GetCurrent();

            #endregion

            #region Methods

            private Token GetCurrent()
            {
                VerifyNotDisposed();

                if (_currentToken == null)
                    throw new Exception($"{nameof(Current)} property is not valid until {nameof(MoveNext)} is called");

                return _currentToken;
            }

            public bool MoveNext()
            {
                VerifyNotDisposed();

                if (_currentToken != null &&
                    _currentToken.Kind == TokenKind.EOF)
                    return false;

                _currentToken = _lex.NextToken(ref _position);
                return true;
            }

            public void Reset()
            {
                VerifyNotDisposed();

                _position = 0;
            }

            public void Dispose()
            {
                _isDisposed = true;
            }

            private void VerifyNotDisposed()
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(nameof(TokenEnumerator));
            }

            #endregion
        }

        #endregion

        #region Construction

        public Lexer(string expression)
        {
            _expression = expression;
        }

        #endregion

        #region Fields

        private readonly string _expression;

        #endregion

        #region Methods

        public IEnumerator<Token> GetEnumerator() =>
            new TokenEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() =>
            new TokenEnumerator(this);

        private Token NextToken(ref int position)
        {
            for (; position < _expression.Length; position++)
            {
                Token? token = null;

                switch (_expression[position])
                {
                    case '+':
                        token = Token.Add(position);
                        position++;
                        break;

                    case '-':
                        token = Token.Substract(position);
                        position++;
                        break;

                    case '*':
                        token = Token.Multiply(position);
                        position++;
                        break;

                    case '/':
                        token = Token.Divide(position);
                        position++;
                        break;

                    case '(':
                        token = Token.OpenBracket(position);
                        position++;
                        break;

                    case ')':
                        token = Token.CloseBracket(position);
                        position++;
                        break;

                    case '=':
                        token = Token.Equals(position);
                        position++;
                        break;

                    case ',':
                        token = Token.Comma(position);
                        position++;
                        break;

                    case '"':
                        {
                            int pos = position;
                            string value = AcceptStringConstant(ref position);
                            token = Token.String(pos, value);
                        }
                        break;

                    default:
                        {
                            if (Char.IsWhiteSpace(_expression[position]))
                                continue;

                            if (Char.IsNumber(_expression[position]))
                            {
                                int pos = position;
                                string number = AcceptNumber(ref position);
                                return Token.Number(pos, number);
                            }

                            if (Char.IsLetter(_expression[position]) || _expression[position] == '_')
                            {
                                int pos = position;
                                string identifier = AcceptIdentifier(ref position);

                                if (String.Equals(identifier, "min", StringComparison.InvariantCultureIgnoreCase))
                                    return Token.Function(pos, identifier);

                                return Token.Identifier(pos, identifier);
                            }

                            throw new UnexpectedCharacterException(_expression, position);
                        }
                }

                if (token != null)
                    return token;
            }

            return Token.EOF;
        }

        private string AcceptNumber(ref int position) =>
            Accept((ch) => Char.IsDigit(ch) || ch == '.', ref position);

        private string AcceptIdentifier(ref int position) =>
            Accept((ch) => Char.IsLetterOrDigit(ch) || ch == '_', ref position);

        private string Accept(Predicate<char> fnAccept, ref int position)
        {
            StringBuilder sb = new StringBuilder();

            while (position < _expression.Length && fnAccept(_expression[position]))
            {
                sb.Append(_expression[position]);
                position++;
            }

            return sb.ToString();
        }

        private string AcceptStringConstant(ref int position)
        {
            StringBuilder sb = new StringBuilder();

            // skip over the initial double quote
            position++;

            while (position < _expression.Length &&
                _expression[position] != '"')
            {
                if (_expression[position] == '\\' && position + 1 < _expression.Length)
                {
                    sb.Append(_expression[position + 1]);
                    position++;
                }
                else
                {
                    sb.Append(_expression[position]);
                }

                position++;
            }

            // skip over the last double quote
            position++;

            return sb.ToString();
        }

        #endregion

    }
}