using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    internal class IdentifierDef
    {
        public IdentifierDef(string name, Func<string, Variant?> fnGetValue)
        {
            this.Name = name;
            this.IsAsync = false;

            _fnGetValue = fnGetValue;
        }

        public IdentifierDef(string name, Func<string, CancellationToken, ValueTask<Variant?>> fnGetValueAsync)
        {
            this.Name = name;
            this.IsAsync = true;

            _fnGetValueAsync = fnGetValueAsync;
        }

        #region Fields

        private readonly Func<string, Variant?>? _fnGetValue;
        private readonly Func<string, CancellationToken, ValueTask<Variant?>>? _fnGetValueAsync;

        #endregion

        #region Properties

        public string Name { get; }
        
        public bool IsAsync { get; }

        #endregion

        #region Methods

        public Variant? GetValue() =>
            this.IsAsync ? throw new NotSupportedException() : _fnGetValue!(this.Name);

        public async ValueTask<Variant?> GetValueAsync(CancellationToken cancellation) =>
            this.IsAsync ? await _fnGetValueAsync!(this.Name, cancellation) : GetValue();

        #endregion

    }
}
