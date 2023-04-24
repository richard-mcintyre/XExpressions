using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class HashCodeTests
    {
        public static TestCaseData[] HashCodeTestCases =
        {
            new TestCaseData(new Variant(123), new Variant(123))
                .SetArgDisplayNames("123m", "123m")
                .Returns(true),
            new TestCaseData(new Variant(123), new Variant(113))
                .SetArgDisplayNames("123m", "113m")
                .Returns(false),
            new TestCaseData(new Variant(0), Variant.False)
                .SetArgDisplayNames("0m", "false")
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

        [Test, TestCaseSource(nameof(HashCodeTestCases))]
        public bool HashCode(Variant left, Variant right) =>
            left.GetHashCode() == right.GetHashCode();
    }
}
