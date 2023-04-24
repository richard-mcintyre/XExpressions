using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.Tests.Fakes;

namespace XExpressions.Tests
{
    public class ParserTest
    {
        [Test]
        public void NumberAddNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20)));

            AssertParserTree(parser, 
                Token.Add(), 
                Token.Number(10), 
                Token.Number(20));
        }

        [Test]
        public void NumberSubtractNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(35),
                Token.Substract(),
                Token.Number(27)));

            AssertParserTree(parser,
                Token.Substract(),
                Token.Number(35),
                Token.Number(27));
        }

        [Test]
        public void NumberMultiplyNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Multiply(),
                Token.Number(20)));

            AssertParserTree(parser,
                Token.Multiply(),
                Token.Number(10),
                Token.Number(20));
        }

        [Test]
        public void NumberDivideNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Divide(),
                Token.Number(2)));

            AssertParserTree(parser,
                Token.Divide(),
                Token.Number(10),
                Token.Number(2));
        }

        [Test]
        public void NumberAddNumberAddNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Add(),
                Token.Number(30)));

            AssertParserTree(parser,
                Token.Add(),
                Token.Add(),
                Token.Number(10),
                Token.Number(20),
                Token.Number(30));
        }

        [Test]
        public void NumberAddNumberSubtractNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Substract(),
                Token.Number(12)));

            AssertParserTree(parser,
                Token.Substract(),
                Token.Add(),
                Token.Number(10),
                Token.Number(20),
                Token.Number(12));
        }

        [Test]
        public void NumberMultiplyNumberDivideNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Multiply(),
                Token.Number(20),
                Token.Divide(),
                Token.Number(2)));

            AssertParserTree(parser,
                Token.Divide(),
                Token.Multiply(),
                Token.Number(10),
                Token.Number(20),
                Token.Number(2));
        }

        [Test]
        public void NumberAddNumberMultiplyNumber()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Number(2),
                Token.Multiply(),
                Token.Number(3)));

            AssertParserTree(parser,
                Token.Add(),
                Token.Number(1),
                Token.Multiply(),
                Token.Number(2),
                Token.Number(3));
        }

        [Test]
        public void Identifier()
        {
            const string identName = "test_ident";

            Parser parser = new Parser(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Identifier(identName)));

            AssertParserTree(parser,
                Token.Add(),
                Token.Number(1),
                Token.Identifier(identName));
        }

        [Test]
        public void StringConstant()
        {
            const string testValue = "abc";

            Parser parser = new Parser(new LexerFake(
                Token.String(testValue)));

            AssertParserTree(parser,
                Token.String(testValue));
        }

        [Test]
        public void IdentStringEqualsString()
        {
            const string identName = "ident1";
            const string testValue = "abc";

            Parser parser = new Parser(new LexerFake(
                Token.Identifier(identName),
                Token.Equals(),
                Token.String(testValue)));

            AssertParserTree(parser,
                Token.Equals(),
                Token.Identifier(identName),
                Token.String(testValue));
        }

        [Test]
        public void Equals()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(10),
                Token.Add(),
                Token.Number(20),
                Token.Equals(),
                Token.Number(30)));

            AssertParserTree(parser,
                Token.Equals(),
                Token.Add(),
                Token.Number(10),
                Token.Number(20),
                Token.Number(30));
        }

        [Test]
        public void Func_Min()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Number(100),
                Token.Comma(),
                Token.Number(20),
                Token.Add(),
                Token.Number(30),
                Token.CloseBracket()));

            AssertParserTree(parser,
                Token.Function("min"),
                Token.Number(100),
                Token.Add(),
                Token.Number(20),
                Token.Number(30));
        }

        [Test]
        public void FuncWithIdents_Min()
        {
            string identName1 = "ident1";
            string identName2 = "ident2";

            Parser parser = new Parser(new LexerFake(
                Token.Function("min"),
                Token.OpenBracket(),
                Token.Identifier(identName1),
                Token.Comma(),
                Token.Identifier(identName2),
                Token.CloseBracket()));

            AssertParserTree(parser,
                Token.Function("min"),
                Token.Identifier(identName1),
                Token.Identifier(identName2));
        }

        [Test]
        public void FuncWithFuncParameter()
        {
            Parser parser = new Parser(new LexerFake(
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

            AssertParserTree(parser,
                Token.Function("min"),
                Token.Function("min"),
                Token.Number(100),
                Token.Number(200),
                Token.Number(400));
        }

        [Test]
        public void UnknownFunction([Values("nonfuncname")] string name)
        {
            Parser parser = new Parser(new LexerFake(
                Token.Function(name),
                Token.OpenBracket(),
                Token.CloseBracket()));

            Assert.Throws<InvalidExpressionException>(() => parser.Evaluate());
        }

        [Test]
        public void MismatchedBrackets()
        {
            Parser parser = new Parser(new LexerFake(
                Token.OpenBracket(),
                Token.Number(1),
                Token.Add(),
                Token.OpenBracket(),
                Token.Number(2),
                Token.Add(),
                Token.Number(3),
                Token.CloseBracket()));

            Assert.Throws<InvalidExpressionException>(() => parser.Evaluate());
        }

        [Test]
        public void IncompleteExpression()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(1),
                Token.Add()));

            Assert.Throws<InvalidExpressionException>(() => parser.Evaluate());
        }

        [Test]
        public void NumberAddOpAddOp()
        {
            Parser parser = new Parser(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Add()));

            Assert.Throws<InvalidExpressionException>(() => parser.Evaluate());
        }

        private static void AssertParserTree(Parser parser, params Token[] expectedTokens)
        {
            ExpressionNode rootNode = parser.Evaluate();

            Token[] actualTokens = WalkTree(rootNode).ToArray();

            CollectionAssert.AreEqual(expectedTokens, actualTokens);
        }

        private static IEnumerable<Token> WalkTree(ExpressionNode node)
        {
            yield return node.Token;

            foreach (ExpressionNode childNode in node.Children)
            {
                foreach (Token childToken in WalkTree(childNode))
                {
                    yield return childToken;
                }
            }
        }
    }
}
