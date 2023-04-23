using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class DataType_BooleanTests
    {
        [Test]
        public void Boolean([Values(false, true)] bool value)
        {
            Variant v = value;

            Assert.That(v.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.That(v.Boolean, Is.EqualTo(value));
        }

        [Test]
        public void Boolean_True()
        {
            Variant v = Variant.True;

            Assert.That(v.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.That(v.Boolean, Is.EqualTo(true));
        }

        [Test]
        public void Boolean_False()
        {
            Variant v = Variant.True;

            Assert.That(v.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.That(v.Boolean, Is.EqualTo(true));
        }

        [Test]
        public void ImplicitConversionFrom_Boolean([Values(true, false)] bool value)
        {
            Variant test = value;

            Assert.That(test.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.That(test.Boolean, Is.EqualTo(value));
        }

        [Test]
        public void ImplicitConversionFrom_Boolean_InvalidCast([Values(100)] decimal value)
        {
            Variant test = value;

            Assert.Throws<InvalidCastException>(() => _ = (bool)test);
        }

        [Test]
        public void ExplicitConversionTo_Boolean([Values(true, false)] bool value)
        {
            Variant test = value;

            bool actual = (bool)test;

            Assert.That(actual, Is.EqualTo(value));
        }

    }
}
