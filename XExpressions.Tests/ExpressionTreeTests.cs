using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.Tests.Fakes;

namespace XExpressions.Tests
{
    public class ExpressionTreeTests
    {
        [Test]
        public void Test()
        {
            Evaluator et = new Evaluator(new LexerFake(
                Token.Number(1),
                Token.Add(),
                Token.Number(2),
                Token.Add(),
                Token.Identifier("pi")));

            var res = et.Evaluate(ident =>
            {
                return 3.14m;
            });

        }
    }
}
