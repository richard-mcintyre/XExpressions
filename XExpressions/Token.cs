using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    public record Token(TokenKind Kind, int Position, string Value)
    {
        public static Token FromVariant(Variant value)
        {
            switch (value.Kind)
            {
                case VariantKind.Boolean:
                    return Token.Boolean(value.Boolean);

                case VariantKind.Decimal:
                    return Token.Number(value.Decimal);

                case VariantKind.String:
                    return Token.String(value.String);
            }

            throw new Exception($"Unsupported data type: {value.Kind}");
        }

        public static readonly Token EOF = new Token(TokenKind.EOF, 0, System.String.Empty);

        public static Token Number(decimal value) =>
            Token.Number(position: -1, value);
        public static Token Number(int position, decimal value) =>
            Token.Number(position, value.ToString(CultureInfo.InvariantCulture));

        public static Token Number(int position, string value) =>
            new Token(TokenKind.Number, position, value);

        public static Token String(string value) =>
            Token.String(position: -1, value);

        public static Token String(int position, string value) =>
            new Token(TokenKind.String, position, value);

        public static Token Boolean(bool value) =>
            Token.Boolean(position: -1, value);

        public static Token Boolean(int position, bool value) =>
            new Token(TokenKind.Boolean, position, value ? System.Boolean.TrueString : System.Boolean.FalseString);

        public static Token Identifier(string identifier) =>
            Token.Identifier(-1, identifier);

        public static Token Identifier(int position, string identifier) =>
            new Token(TokenKind.Identifier, position, identifier);

        public static Token Add() => 
            Token.Add(position: -1);

        public static Token Add(int position) => 
            new Token(TokenKind.AddOperator, position, "+");

        public static Token Substract() =>
            Token.Substract(position: -1);

        public static Token Substract(int position) => 
            new Token(TokenKind.SubtractOperator, position, "-");

        public static Token Multiply() =>
            Token.Multiply(position: -1);

        public static Token Multiply(int position) => 
            new Token(TokenKind.MultiplyOperator, position, "*");

        public static Token Divide() =>
            Token.Divide(position: -1);

        public static Token Divide(int position) => 
            new Token(TokenKind.DivideOperator, position, "/");

        public static Token Equals() =>
            Token.Equals(position: -1);

        public static Token Equals(int position) => 
            new Token(TokenKind.Equals, position, "=");

        public static Token OpenBracket() =>
            Token.OpenBracket(position: -1);

        public static Token OpenBracket(int position) => 
            new Token(TokenKind.OpenBracket, position, "(");

        public static Token CloseBracket() =>
            Token.CloseBracket(position: -1);

        public static Token CloseBracket(int position) => 
            new Token(TokenKind.CloseBracket, position, ")");

        public static Token Function(string name) =>
            Token.Function(position: -1, name);

        public static Token Function(int position, string name) =>
            new Token(TokenKind.Function, position, name);

        public static Token Comma() =>
            Token.Comma(position: -1);

        public static Token Comma(int position) =>
            new Token(TokenKind.Comma, position, ",");

        public Variant GetValue()
        {
            switch (this.Kind)
            {
                case TokenKind.Number:
                    return Convert.ToDecimal(this.Value);

                case TokenKind.String:
                    return this.Value;

                case TokenKind.Boolean:
                    return Convert.ToBoolean(this.Value);
            }

            throw new Exception($"Token {this.Kind} does not represent a value");
        }

        public bool IsOperatorOrEquals =>
            (this.Kind == TokenKind.AddOperator ||
             this.Kind == TokenKind.SubtractOperator ||
             this.Kind == TokenKind.MultiplyOperator ||
             this.Kind == TokenKind.DivideOperator ||
             this.Kind == TokenKind.Equals);

        public bool IsValueOrIdentifier =>
            (this.Kind == TokenKind.Boolean ||
             this.Kind == TokenKind.Number ||
             this.Kind == TokenKind.String ||
             this.Kind == TokenKind.Identifier);
    }
}
