using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class DataType_DecimalTests
    {
        public static TestCaseData[] DecimalTestCases =
        {
            new TestCaseData(100m),
            new TestCaseData(-100m),
            new TestCaseData(decimal.MaxValue),
            new TestCaseData(decimal.MinValue),
        };

        [Test, TestCaseSource(nameof(DecimalTestCases))]
        public void Construction(decimal value)
        {
            Variant v = new Variant(value);

            Assert.That(v.Kind, Is.EqualTo(VariantKind.Decimal));
            Assert.That(v.Decimal, Is.EqualTo(value));
        }

        [Test]
        public void ImplicitConversionFrom_Decimal([Values(100, -100)] decimal value)
        {
            Variant test = value;

            Assert.That(test.Kind, Is.EqualTo(VariantKind.Decimal));
            Assert.That(test.Decimal, Is.EqualTo(value));
        }

        [Test]
        public void ImplicitConversionFrom_Decimal_InvalidCast([Values(true, false)] bool value)
        {
            Variant test = value;

            Assert.Throws<InvalidCastException>(() => _ = (decimal)test);
        }

        [Test]
        public void ExplicitConversionTo_Decimal([Values(100, -100)] decimal value)
        {
            Variant test = value;

            decimal actual = (decimal)test;

            Assert.That(actual, Is.EqualTo(value));
        }
    }
}
