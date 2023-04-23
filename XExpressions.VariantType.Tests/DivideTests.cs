using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace XExpressions.VariantType.Tests
{
    public class DivideTests
    {
        public static TestCaseData[] DivideTestCases =
{
            new TestCaseData(new Variant(100m), new Variant(25m))
                .SetArgDisplayNames("100m", "25m")
                .Returns(new Variant(4m)),
        };

        [Test, TestCaseSource(nameof(DivideTestCases))]
        public Variant Divide(Variant left, Variant right) => left / right;



        public static TestCaseData[] DivideFailuresTestCases =
        {
            new TestCaseData(Variant.True, Variant.True)
                .SetArgDisplayNames("true", "true"),
            new TestCaseData(Variant.False, Variant.False)
                .SetArgDisplayNames("false", "false"),
            new TestCaseData(Variant.True, Variant.False)
                .SetArgDisplayNames("true", "false"),
            new TestCaseData(Variant.False, Variant.True)
                .SetArgDisplayNames("false", "true"),
            new TestCaseData(new Variant(100m), Variant.True)
                .SetArgDisplayNames("100m", "true"),
            new TestCaseData(new Variant("abc"), new Variant("def"))
                .SetArgDisplayNames("\"abc\"", "\"def\""),
            new TestCaseData(new Variant("abc"), new Variant(100m))
                .SetArgDisplayNames("\"abc\"", "100m"),
            new TestCaseData(new Variant("abc"), Variant.True)
                .SetArgDisplayNames("\"abc\"", "true"),
        };

        [Test, TestCaseSource(nameof(DivideFailuresTestCases))]
        public void DivideFailures(Variant left, Variant right) =>
            Assert.Throws<Exception>(() => _ = left / right);
    }
}
