using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions.Tests
{
    public class LexerTests
    {
        [Test]
        public void NumberToken([Values("1", "123", "123.45")] string expression)
        {
            Lexer lex = new Lexer(expression);

            Token[] tokens = lex.ToArray();
            
            CollectionAssert.AreEqual(tokens, new Token[] { Token.Number(0, expression), Token.EOF });
        }

        [Test]
        public void StringToken([Values("abc", "embedded\\\"double quote")] string expression)
        {
            Lexer lex = new Lexer($"\"{expression}\"");

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] { Token.String(0, expression.Replace("\\\"", "\"")), Token.EOF });
        }

        [Test, Sequential]
        public void OperatorToken([Values("+", "-", "*", "/")] string expression,
                                  [Values(TokenKind.AddOperator, TokenKind.SubtractOperator, TokenKind.MultiplyOperator, TokenKind.DivideOperator)] TokenKind kind)
        {
            Lexer lex = new Lexer(expression);

            Token[] tokens = lex.ToArray();

            switch (kind)
            {
                case TokenKind.AddOperator:
                    CollectionAssert.AreEqual(tokens, new Token[] { Token.Add(0), Token.EOF });
                    break;

                case TokenKind.SubtractOperator:
                    CollectionAssert.AreEqual(tokens, new Token[] { Token.Substract(0), Token.EOF });
                    break;

                case TokenKind.MultiplyOperator:
                    CollectionAssert.AreEqual(tokens, new Token[] { Token.Multiply(0), Token.EOF });
                    break;

                case TokenKind.DivideOperator:
                    CollectionAssert.AreEqual(tokens, new Token[] { Token.Divide(0), Token.EOF });
                    break;
            }
        }

        [Test]
        public void BracketTokens()
        {
            Lexer lex = new Lexer("( )");

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] 
            { 
                Token.OpenBracket(0),
                Token.CloseBracket(2),
                Token.EOF
            });
        }

        [Test]
        public void EqualsToken()
        {
            Lexer lex = new Lexer("=");

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] { Token.Equals(0), Token.EOF });
        }

        [Test]
        public void IdentifierToken([Values("abc123", "_123", "_a_b_c_1_2_3")] string expression)
        {
            Lexer lex = new Lexer(expression);

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] { Token.Identifier(0, expression), Token.EOF });
        }

        [Test]
        public void EOFToken()
        {
            Lexer lex = new Lexer(String.Empty);

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] { Token.EOF });
        }

        [Test]
        public void TokenPosition()
        {
            const string expression = "   123";

            Lexer lex = new Lexer(expression);

            Token[] tokens = lex.ToArray();

            CollectionAssert.AreEqual(tokens, new Token[] { Token.Number(3, expression.Trim()), Token.EOF });
        }

        [Test]
        public void MultipleTokens()
        {
            Lexer lex = new Lexer("1 + 2 - 3 * 4 / 5 + (10 + 20) = 123.45");

            var expected = new[]
            {
                new Token(TokenKind.Number, 0, "1"),
                new Token(TokenKind.AddOperator, 2, "+"),
                new Token(TokenKind.Number, 4, "2"),
                new Token(TokenKind.SubtractOperator, 6, "-"),
                new Token(TokenKind.Number, 8, "3"),
                new Token(TokenKind.MultiplyOperator, 10, "*"),
                new Token(TokenKind.Number, 12, "4"),
                new Token(TokenKind.DivideOperator, 14, "/"),
                new Token(TokenKind.Number, 16, "5"),
                new Token(TokenKind.AddOperator, 18, "+"),
                new Token(TokenKind.OpenBracket, 20, "("),
                new Token(TokenKind.Number, 21, "10"),
                new Token(TokenKind.AddOperator, 24, "+"),
                new Token(TokenKind.Number, 26, "20"),
                new Token(TokenKind.CloseBracket, 28, ")"),
                new Token(TokenKind.Equals, 30, "="),
                new Token(TokenKind.Number, 32, "123.45"),
                Token.EOF
            };

            Token[] actual = lex.ToArray();

            CollectionAssert.AreEqual(expected, actual);

        }

        [Test]
        public void UnexpectedCharacter()
        {
            const string expression = "]";

            Lexer lex = new Lexer(expression);

            UnexpectedCharacterException ex = Assert.Throws<UnexpectedCharacterException>(() => lex.ToArray());
            Assert.That(ex.Message, Is.EqualTo($"Unexpected character: {expression[0]}"));
            Assert.That(ex.Expression, Is.EqualTo(expression));
            Assert.That(ex.Position, Is.EqualTo(0));
            Assert.That(ex.Character, Is.EqualTo(expression[0]));
        }

        [Test]
        public void UnexpectedCharacterInTokens()
        {
            const string expression = "1 + 2 + ] + 3";

            Lexer lex = new Lexer(expression);

            using (IEnumerator<Token> enumerator = lex.GetEnumerator())
            {
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.MoveNext());
                Assert.DoesNotThrow(() => enumerator.MoveNext());

                UnexpectedCharacterException ex = Assert.Throws<UnexpectedCharacterException>(() => enumerator.MoveNext());
                Assert.That(ex.Message, Is.EqualTo($"Unexpected character: ]"));
                Assert.That(ex.Expression, Is.EqualTo(expression));
                Assert.That(ex.Position, Is.EqualTo(8));
                Assert.That(ex.Character, Is.EqualTo(']'));
            }
        }

        [Test]
        public void Enumerator()
        {
            const string expression = "1 + 2";

            var expected = new[]
            {
                new Token(TokenKind.Number, 0, "1"),
                new Token(TokenKind.AddOperator, 2, "+"),
                new Token(TokenKind.Number, 4, "2"),
                Token.EOF
            };

            Lexer lex = new Lexer(expression);

            using (IEnumerator<Token> enumerator = lex.GetEnumerator())
            {
                foreach (Token expectedToken in expected)
                {
                    Assert.IsTrue(enumerator.MoveNext());
                    Assert.That(enumerator.Current, Is.EqualTo(expectedToken));
                }

                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [Test]
        public void EnumeratorReset()
        {
            const string expression = "1 + 2";

            var expected = new[]
            {
                new Token(TokenKind.Number, 0, "1"),
                new Token(TokenKind.AddOperator, 2, "+"),
                new Token(TokenKind.Number, 4, "2"),
                Token.EOF
            };

            Lexer lex = new Lexer(expression);

            using (IEnumerator<Token> enumerator = lex.GetEnumerator())
            {
                enumerator.MoveNext();
                enumerator.MoveNext();
                enumerator.Reset();

                foreach (Token expectedToken in expected)
                {
                    Assert.IsTrue(enumerator.MoveNext());
                    Assert.That(enumerator.Current, Is.EqualTo(expectedToken));
                }

                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [Test]
        public void EnumeratorCurrentBeforeMoveNext()
        {
            Lexer lex = new Lexer("1 + 2");

            using (IEnumerator<Token> enumerator = lex.GetEnumerator())
            {
                Assert.Throws<Exception>(() => _ = enumerator.Current);
            }
        }

        [Test]
        public void EnumeratorDisposed()
        {
            Lexer lex = new Lexer("1 + 2");

            IEnumerator<Token> enumerator = lex.GetEnumerator();
            enumerator.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _ = enumerator.Current);
        }
    }
}