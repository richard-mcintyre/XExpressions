using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExpressions.Tests.Fakes
{
    internal class LexerFake : ILexer
    {
        #region Construction

        public LexerFake(params Token[] tokens)
        {
            _tokens.AddRange(tokens);
        }

        #endregion

        #region Fields

        private readonly List<Token> _tokens = new List<Token>();

        #endregion

        #region Methods

        public IEnumerator<Token> GetEnumerator() => _tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _tokens.GetEnumerator();

        #endregion
    }
}
