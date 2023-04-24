using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XExpressions.VariantType;

namespace XExpressions.Tests
{
    public class LexerAndPaserTests
    {
        static LexerAndPaserTests()
        {
            _identifierValues.Add("ident1", new Variant(Ident1Value));
            _identifierValues.Add("ident2", new Variant(Ident2Value));

            foreach (string name in _identifierValues.Keys)
                _settings.AddIdentifier(name, GetIdentifierValue);
        }

        private static Dictionary<string, Variant> _identifierValues = new Dictionary<string, Variant>();
        private static decimal Ident1Value = 123m;
        private static decimal Ident2Value = 234.56m;
        private static XExpressionsSettings _settings = new XExpressionsSettings();

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
            new TestCaseData("max(20, 30)").Returns(Math.Max(20, 30)),
            new TestCaseData("ident1").Returns(Ident1Value),
            new TestCaseData("ident1 * ident2").Returns(Ident1Value * Ident2Value),
        };

        [Test, TestCaseSource(nameof(DecimalResultTestCases))]
        public decimal DecimalResultTests(string expression)
        {
            Evaluator evaluator = new Evaluator(expression, _settings);

            Variant result = evaluator.Evaluate();

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
            Evaluator evaluator = new Evaluator(expression, _settings);

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Boolean));

            return (bool)result;
        }

        [Test]
        public void CustomFunction()
        {
            string functionName = "test_func_name";

            Variant[] expectedArgs = new Variant[] { 10, 20, 30 };
            Variant expectedResult = 123;
            Variant[]? actualArgs = null;

            XExpressionsSettings settings = new XExpressionsSettings();
            settings.AddFunction(functionName, 3, (name, args) =>
            {
                actualArgs = args;
                return expectedResult;
            });

            Evaluator evaluator = new Evaluator($"{functionName}({(decimal)expectedArgs[0]}, {(decimal)expectedArgs[1]}, {(decimal)expectedArgs[2]})", settings);

            Variant actualResult = evaluator.Evaluate();

            Assert.That(actualResult, Is.EqualTo(expectedResult));
            CollectionAssert.AreEqual(expectedArgs, actualArgs);
        }

        [Test]
        public async Task AsyncIdentifierUsingEvaluateAsync()
        {
            string identName = "test_indent";

            XExpressionsSettings settings = new XExpressionsSettings();
            settings.AddIdentifier(identName, async (name, cancellation) =>
            {
                await Task.Delay(1);
                return 123;
            });

            Evaluator evaluator = new Evaluator($"{identName} = 123", settings);

            Variant actualResult = await evaluator.EvaluateAsync();

            Assert.That(actualResult.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.IsTrue(actualResult.Boolean);
        }

        [Test]
        public void AsyncIdentifierUsingEvaluate()
        {
            string identName = "test_indent";

            XExpressionsSettings settings = new XExpressionsSettings();
            settings.AddIdentifier(identName, async (name, cancellation) =>
            {
                await Task.Delay(1);
                return 123;
            });

            Evaluator evaluator = new Evaluator($"{identName} = 123", settings);

            Variant actualResult = evaluator.Evaluate();

            Assert.That(actualResult.Kind, Is.EqualTo(VariantKind.Boolean));
            Assert.IsTrue(actualResult.Boolean);
        }

        [Test]
        public async Task AsyncFunctionUsingEvaluatAsync()
        {
            string funcName = "test_func_name";

            XExpressionsSettings settings = new XExpressionsSettings();
            settings.AddFunction(funcName, 2, async (name, args, cancellation) =>
            {
                await Task.Delay(1);
                return args[0] + args[1];
            });

            Evaluator evaluator = new Evaluator($"{funcName}(100, 200)", settings);

            Variant actualResult = await evaluator.EvaluateAsync();

            Assert.That(actualResult.Kind, Is.EqualTo(VariantKind.Decimal));
            Assert.That(actualResult.Decimal, Is.EqualTo(300m));
        }

        [Test]
        public void AsyncFunctionUsingEvaluatSync()
        {
            string funcName = "test_func_name";

            XExpressionsSettings settings = new XExpressionsSettings();
            settings.AddFunction(funcName, 2, async (name, args, cancellation) =>
            {
                await Task.Delay(1);
                return args[0] + args[1];
            });

            Evaluator evaluator = new Evaluator($"{funcName}(100, 200)", settings);

            Variant actualResult = evaluator.Evaluate();

            Assert.That(actualResult.Kind, Is.EqualTo(VariantKind.Decimal));
            Assert.That(actualResult.Decimal, Is.EqualTo(300m));
        }

        [Test]
        public async Task ___()
        {
            XExpressionsSettings settings = new XExpressionsSettings();

            // Add a function that calls a REST API
            settings.AddFunction(name: "MyFunc", parameterCount: 0,
                async(name, args, cancellation) =>
                {
                    // ... call a REST API and return a result ...
                    await Task.Delay(1);
                    return 123m;
                });

            // Evaluate the expression
            Evaluator eval = new Evaluator("myfunc()", settings);
            Variant result = await eval.EvaluateAsync();
        }
    }
}
