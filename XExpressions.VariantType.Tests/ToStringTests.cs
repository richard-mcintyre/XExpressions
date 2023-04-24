using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class ToStringTests
    {
        public static TestCaseData[] ToStringTestCases =
        {
            new TestCaseData(new Variant(100m))
                .SetArgDisplayNames("100m")
                .Returns(GetExpectedToString(VariantKind.Decimal, 100)),
            new TestCaseData(new Variant(-100m))
                .SetArgDisplayNames("-100m")
                .Returns(GetExpectedToString(VariantKind.Decimal, -100)),
            new TestCaseData(Variant.True)
                .SetArgDisplayNames("true")
                .Returns(GetExpectedToString(VariantKind.Boolean, true)),
            new TestCaseData(Variant.False)
                .SetArgDisplayNames("false")
                .Returns(GetExpectedToString(VariantKind.Boolean, false)),
            new TestCaseData(new Variant("abc"))
                .SetArgDisplayNames("\"abc\"")
                .Returns(GetExpectedToString(VariantKind.String, "abc")),
        };

        [Test, TestCaseSource(nameof(ToStringTestCases))]
        public string ToString(Variant value) => $"-->{value}<--";

        private static string GetExpectedToString(VariantKind kind, object value) => $"-->{value} ({kind})<--";
    }
}
