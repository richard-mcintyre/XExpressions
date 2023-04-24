using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class EqualsTests
    {
        public static TestCaseData[] EqualsTestCases =
        {
            new TestCaseData(new Variant(100m), new Variant(100m))
                .SetArgDisplayNames("100m", "100m")
                .Returns(true),
            new TestCaseData(new Variant(100m), new Variant(101m))
                .SetArgDisplayNames("100m", "101m")
                .Returns(false),
            new TestCaseData(new Variant(true), new Variant(true))
                .SetArgDisplayNames("true", "true")
                .Returns(true),
            new TestCaseData(new Variant(false), new Variant(false))
                .SetArgDisplayNames("false", "false")
                .Returns(true),
            new TestCaseData(new Variant(true), new Variant(false))
                .SetArgDisplayNames("true", "false")
                .Returns(false),
            new TestCaseData(new Variant(true), null)
                .SetArgDisplayNames("true", "null")
                .Returns(false),
            new TestCaseData(null, new Variant(true))
                .SetArgDisplayNames("null", "true")
                .Returns(false),
            new TestCaseData(new Variant(100m), new Variant(true))
                .SetArgDisplayNames("100m", "true")
                .Returns(false),
            new TestCaseData(new Variant("abc"), new Variant("def"))
                .SetArgDisplayNames("\"abc\"", "\"def\"")
                .Returns(false),
            new TestCaseData(new Variant("abc"), new Variant("abc"))
                .SetArgDisplayNames("\"abc\"", "\"abc\"")
                .Returns(true),
            new TestCaseData(new Variant("abc"), new Variant(100m))
                .SetArgDisplayNames("\"abc\"", "100m")
                .Returns(false),
            new TestCaseData(new Variant("abc"), Variant.True)
                .SetArgDisplayNames("\"abc\"", "true")
                .Returns(false),
        };

        [Test, TestCaseSource(nameof(EqualsTestCases))]
        public bool Equals(Variant left, Variant right) =>
            left.Equals(right);

        [Test, TestCaseSource(nameof(EqualsTestCases))]
        public bool ObjectEquals(object left, object right)
        {
            if (left == null)
            {
                Assert.Pass();
                return false;
            }

            return left.Equals(right);
        }

        [Test, TestCaseSource(nameof(EqualsTestCases))]
        public bool EqualsOperator(Variant left, Variant right) => (left == right);

        [Test, TestCaseSource(nameof(EqualsTestCases))]
        public bool NotEqualsOperator(Variant left, Variant right) => !(left != right);
    }
}
