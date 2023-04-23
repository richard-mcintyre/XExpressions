using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class DataType_StringTests
    {
        public static TestCaseData[] StringTestCases =
        {
            new TestCaseData("abc"),
        };

        [Test, TestCaseSource(nameof(StringTestCases))]
        public void Construction(string value)
        {
            Variant v = new Variant(value);

            Assert.That(v.Kind, Is.EqualTo(VariantKind.String));
            Assert.That(v.String, Is.EqualTo(value));
        }

        [Test]
        public void ImplicitConversionFrom_String([Values("abc")] string value)
        {
            Variant test = value;

            Assert.That(test.Kind, Is.EqualTo(VariantKind.String));
            Assert.That(test.String, Is.EqualTo(value));
        }

        [Test]
        public void ImplicitConversionFrom_String_InvalidCast([Values(true, false)] bool value)
        {
            Variant test = value;

            Assert.Throws<InvalidCastException>(() => _ = (string)test);
        }

        [Test]
        public void ExplicitConversionTo_String([Values("abc")] string value)
        {
            Variant test = value;

            string actual = (string)test;

            Assert.That(actual, Is.EqualTo(value));
        }
    }

}
