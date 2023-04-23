using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using XExpressions.VariantType;

namespace XExpressions.Tests
{
    public class LexerAndPaserTests
    {
        static LexerAndPaserTests()
        {
            _identifierValues.Add("ident1", new Variant(Ident1Value));
            _identifierValues.Add("ident2", new Variant(Ident2Value));
        }

        private static Dictionary<string, Variant> _identifierValues = new Dictionary<string, Variant>();
        private static decimal Ident1Value = 123m;
        private static decimal Ident2Value = 234.56m;

        private static Variant? GetIdentifierValue(string name)
        {
            if (_identifierValues.TryGetValue(name, out Variant variant))
                return variant;

            return null;
        }

        public static TestCaseData[] DecimalResultTestCases =
        {
            new TestCaseData("1 + 2").Returns(1m + 2m),
            new TestCaseData("10 - 2").Returns(10m - 2m),
            new TestCaseData("4 * 6").Returns(4m * 6m),
            new TestCaseData("100 / 3").Returns(100m / 3m),
            new TestCaseData("5 + 6 - 7 * 8 / 9").Returns(5m + 6m - 7m * 8m / 9m),
            new TestCaseData("5.6 + 6.6 - 7.6 * 8.6 / 9.6").Returns(5.6m + 6.6m - 7.6m * 8.6m / 9.6m),
            new TestCaseData("min(20, 30)").Returns(Math.Min(20, 30)),
            new TestCaseData("min(20, 30) + 5").Returns(Math.Min(20, 30) + 5),
            new TestCaseData("min(min(20, 30/2), min(100, 101))").Returns(Math.Min(Math.Min(20, 30m / 2m), Math.Min(100, 101))),
            new TestCaseData("min(ident1, ident2)").Returns(Math.Min(Ident1Value, Ident2Value)),
            new TestCaseData("ident1").Returns(Ident1Value),
            new TestCaseData("ident1 * ident2").Returns(Ident1Value * Ident2Value),
        };

        [Test, TestCaseSource(nameof(DecimalResultTestCases))]
        public decimal DecimalResultTests(string expression)
        {
            Evaluator evaluator = new Evaluator(expression);

            Variant result = evaluator.Evaluate(GetIdentifierValue);

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            return (decimal)result;
        }

        public static TestCaseData[] BooleanResultTestCases =
        {
            new TestCaseData("\"abc\" = \"abc\"").Returns(true),
            new TestCaseData("\"abc\" = \"def\"").Returns(false),
        };

        [Test, TestCaseSource(nameof(BooleanResultTestCases))]
        public bool BooleanResultTests(string expression)
        {
            Evaluator evaluator = new Evaluator(expression);

            Variant result = evaluator.Evaluate(GetIdentifierValue);

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Boolean));

            return (bool)result;
        }
    }
}
