using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class AddTests
    {
        public static TestCaseData[] AddTestCases =
        {
            new TestCaseData(new Variant(100m), new Variant(10m))
                .SetArgDisplayNames("100m", "10m")
                .Returns(new Variant(110m)),
            new TestCaseData(Variant.True, Variant.True)
                .SetArgDisplayNames("true", "true")
                .Returns(Variant.True),
            new TestCaseData(Variant.False, Variant.False)
                .SetArgDisplayNames("false", "false")
                .Returns(Variant.False),
            new TestCaseData(Variant.True, Variant.False)
                .SetArgDisplayNames("true", "false")
                .Returns(Variant.False),
            new TestCaseData(Variant.False, Variant.True)
                .SetArgDisplayNames("false", "true")
                .Returns(Variant.False),
        };

        [Test, TestCaseSource(nameof(AddTestCases))]
        public Variant Add(Variant left, Variant right) => left + right;


        public static TestCaseData[] AddFailuresTestCases =
        {
            new TestCaseData(new Variant(100m), Variant.True)
                .SetArgDisplayNames("100m", "true"),
            new TestCaseData(new Variant("abc"), new Variant("def"))
                .SetArgDisplayNames("\"abc\"", "\"def\""),
            new TestCaseData(new Variant("abc"), new Variant(100m))
                .SetArgDisplayNames("\"abc\"", "100m"),
            new TestCaseData(new Variant("abc"), Variant.True)
                .SetArgDisplayNames("\"abc\"", "true"),
        };

        [Test, TestCaseSource(nameof(AddFailuresTestCases))]
        public void AddFailures(Variant left, Variant right) =>
            Assert.Throws<Exception>(() => _ = left + right);
    }
}
