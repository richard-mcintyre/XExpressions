using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions.VariantType
{
    public readonly struct Variant : IEquatable<Variant>
    {
        #region Construction

        public Variant(decimal value)
        {
            _kind = VariantKind.Decimal;

            _data = new byte[sizeof(decimal)];
            Write(value, ref _data);
        }

        public Variant(bool value)
        {
            _kind = VariantKind.Boolean;

            _data = new byte[sizeof(bool)];
            Write(value, ref _data);
        }

        public Variant(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _kind = VariantKind.String;

            _data = new byte[Encoding.UTF8.GetByteCount(value)];
            Encoding.UTF8.GetBytes(value, _data);
        }

        #endregion

        #region Fields

        private readonly VariantKind _kind;
        private readonly byte[] _data;

        #endregion

        #region Properties

        public VariantKind Kind => _kind;

        public bool Boolean => MemoryMarshal.Read<bool>(_data);

        public decimal Decimal => MemoryMarshal.Read<decimal>(_data);

        public string String => Encoding.UTF8.GetString(_data);

        public static Variant True { get; } = new Variant(true);

        public static Variant False { get; } = new Variant(false);

        #endregion

        #region Methods

        private static void Write<T>(T value, ref byte[] data)
            where T : struct
        {
            Span<byte> buffer = new Span<byte>(data);
            MemoryMarshal.Write(buffer, ref value);
        }

        public bool Equals(Variant other)
        {
            if (_data == null || other._data == null)
                return false;

            if (_kind != other.Kind)
                return false;

            return _data.SequenceEqual(other._data);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || !(obj is Variant))
                return false;

            return Equals((Variant)obj);
        }

        public override int GetHashCode()
        {
            HashCode hc = new HashCode();
            hc.Add(_kind);
            hc.AddBytes(_data);

            return hc.ToHashCode();
        }

        public static bool operator ==(Variant left, Variant right) => left.Equals(right);

        public static bool operator !=(Variant left, Variant right) => !left.Equals(right);

        public static Variant operator +(Variant left, Variant right)
        {
            if (left.Kind == right.Kind)
            {
                switch (left.Kind)
                {
                    case VariantKind.Boolean:
                        return new Variant(left.Boolean && right.Boolean);

                    case VariantKind.Decimal:
                        return new Variant(left.Decimal + right.Decimal);
                }
            }

            throw new Exception($"{left.Kind} and {right.Kind} are incompatible for addition");
        }

        public static Variant operator -(Variant left, Variant right)
        {
            if (left.Kind == right.Kind)
            {
                switch (left.Kind)
                {
                    case VariantKind.Decimal:
                        return new Variant(left.Decimal - right.Decimal);
                }
            }

            throw new Exception($"{left.Kind} and {right.Kind} are incompatible for subtraction");
        }

        public static Variant operator *(Variant left, Variant right)
        {
            if (left.Kind == right.Kind)
            {
                switch (left.Kind)
                {
                    case VariantKind.Decimal:
                        return new Variant(left.Decimal * right.Decimal);
                }
            }

            throw new Exception($"{left.Kind} and {right.Kind} are incompatible for multiplication");
        }

        public static Variant operator /(Variant left, Variant right)
        {
            if (left.Kind == right.Kind)
            {
                switch (left.Kind)
                {
                    case VariantKind.Decimal:
                        return new Variant(left.Decimal / right.Decimal);
                }
            }

            throw new Exception($"{left.Kind} and {right.Kind} are incompatible for division");
        }

        public static implicit operator Variant(decimal value) => new Variant(value);

        public static explicit operator decimal(Variant value)
        {
            if (value.Kind != VariantKind.Decimal)
                throw new InvalidCastException();

            return value.Decimal;
        }

        public static implicit operator Variant(bool value) => value ? Variant.True : Variant.False;

        public static explicit operator bool(Variant value)
        {
            if(value.Kind != VariantKind.Boolean)
                throw new InvalidCastException();

            return value.Boolean;
        }

        public static implicit operator Variant(string value) => new Variant(value);

        public static explicit operator string(Variant value)
        {
            if (value.Kind != VariantKind.String)
                throw new InvalidCastException();

            return value.String;
        }

        public override string ToString()
        {
            switch (_kind)
            {
                case VariantKind.Boolean:
                    return $"{this.Boolean} ({_kind})";

                case VariantKind.Decimal:
                    return $"{this.Decimal} ({_kind})";

                case VariantKind.String:
                    return $"{this.String} ({_kind})";
            }

            throw new Exception($"Unknown kind: {_kind}");
        }

        #endregion
    }
}