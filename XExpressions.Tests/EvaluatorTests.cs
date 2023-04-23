using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.Tests.Fakes;
using XExpressions.VariantType;

namespace XExpressions.Tests
{
    public class EvaluatorTests
    {
        [Test]
        public void NumberAddNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(30));
        }

        [Test]
        public void NumberSubtractNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(35),
                Token.Substract(),
                Token.Number(27)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(8));
        }

        [Test]
        public void NumberMultiplyNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Multiply(),
                Token.Number(20)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(200));
        }

        [Test]
        public void NumberDivideNumber()
        {
            Evaluator evalautor = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Divide(),
                Token.Number(2)));

            Variant result = evalautor.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(5));
        }

        [Test]
        public void NumberAddNumberAddNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Add(),
                Token.Number(30)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(60));
        }

        [Test]
        public void NumberAddNumberSubtractNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Substract(),
                Token.Number(12)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(18));
        }

        [Test]
        public void NumberMultiplyNumberDivideNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Multiply(),
                Token.Number(20),
                Token.Divide(),
                Token.Number(2)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(100));
        }

        [Test]
        public void NumberAddNumberMultiplyNumber()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Number(2),
                Token.Multiply(),
                Token.Number(3)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(7m));
        }

        [Test]
        public void Identifier()
        {
            const string identName = "test_ident";

            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Identifier(identName)));

            Variant result = evaluator.Evaluate(o =>
            {
                Assert.That(o, Is.EqualTo(identName));
                return 9;
            });

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(10m));
        }

        [Test]
        public void StringConstant()
        {
            const string testValue = "abc";

            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.String(testValue)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.String));

            string actualValue = (string)result;

            Assert.That(actualValue, Is.EqualTo(testValue));
        }

        [Test]
        public void IdentStringEqualsString()
        {
            const string identName = "ident1";
            const string testValue = "abc";

            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Identifier(identName),
                Token.Equals(),
                Token.String(testValue)));

            Variant result = evaluator.Evaluate(o => testValue);

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Boolean));

            bool actualValue = (bool)result;

            Assert.That(actualValue, Is.True);
        }

        [Test]
        public void Equals()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Equals(),
                Token.Number(30)));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Boolean));

            bool actualValue = (bool)result;

            Assert.That(actualValue, Is.EqualTo(true));
        }

        [Test]
        public void Func_Min()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Number(100),
                Token.Comma(),
                Token.Number(20),
                Token.Add(),
                Token.Number(30),
                Token.CloseBracket()));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(50m));
        }

        [Test]
        public void FuncWithIdents_Min()
        {
            string identName1 = "ident1";
            string identName2 = "ident2";

            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Identifier(identName1),
                Token.Comma(),
                Token.Identifier(identName2),
                Token.CloseBracket()));

            Variant result = evaluator.Evaluate(o =>
            {
                if (String.Equals(o, identName1, StringComparison.Ordinal))
                    return 100;

                if (String.Equals(o, identName2, StringComparison.Ordinal))
                    return 200;

                throw new Exception($"Unknown identifier: {o}");
            });

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(100m));
        }

        [Test]
        public void FuncWithFuncParameter()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Number(100),
                Token.Comma(),
                Token.Number(200),
                Token.CloseBracket(),
                Token.Comma(),
                Token.Number(400),
                Token.CloseBracket()));

            Variant result = evaluator.Evaluate();

            Assert.That(result.Kind, Is.EqualTo(VariantKind.Decimal));

            decimal actualValue = (decimal)result;

            Assert.That(actualValue, Is.EqualTo(100m));
        }

        [Test]
        public void UnknownFunction([Values("nonfuncname")] string name)
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Function(name),
                Token.OpenBracket(),
                Token.CloseBracket()));

            Assert.Throws<InvalidExpressionException>(() => evaluator.Evaluate());
        }

        [Test]
        public void MismatchedBrackets()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.OpenBracket(),
                Token.Number(1),
                Token.Add(),
                Token.OpenBracket(),
                Token.Number(2),
                Token.Add(),
                Token.Number(3),
                Token.CloseBracket()));

            Assert.Throws<InvalidExpressionException>(() => evaluator.Evaluate());
        }

        [Test]
        public void UnknownIdentifier()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Identifier("unknownident")));

            Assert.Throws<InvalidExpressionException>(() => evaluator.Evaluate());
        }

        [Test]
        public void IncompleteExpression()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add()));

            Assert.Throws<InvalidExpressionException>(() => evaluator.Evaluate());
        }

        [Test]
        public void NumberAddOpAddOp()
        {
            Evaluator evaluator = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Add()));

            Assert.Throws<InvalidExpressionException>(() => evaluator.Evaluate());
        }
    }
}
