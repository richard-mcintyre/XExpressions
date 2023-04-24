using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    internal class FunctionDef
    {
        #region Construction

        public FunctionDef(string name, int parameterCount, Func<string, Variant[], Variant> fnFuncImplementation)
        {
            this.Name = name;
            this.ParameterCount = parameterCount;
            this.IsAsync = false;

            _fnFuncImplementation = fnFuncImplementation;
        }

        public FunctionDef(string name, int parameterCount, Func<string, Variant[], CancellationToken, ValueTask<Variant>> fnFuncImplementationAsync)
        {
            this.Name = name;
            this.ParameterCount = parameterCount;
            this.IsAsync = true;

            _fnFuncImplementationAsync = fnFuncImplementationAsync;
        }

        #endregion

        #region Fields

        private readonly Func<string, Variant[], Variant>? _fnFuncImplementation;
        private readonly Func<string, Variant[], CancellationToken, ValueTask<Variant>>? _fnFuncImplementationAsync;

        #endregion

        #region Properties

        public string Name { get; }

        public int ParameterCount { get; }

        public bool IsAsync { get; }

        #endregion

        #region Methods

        public Variant Invoke(Variant[] args) =>
            this.IsAsync ? throw new NotSupportedException() : _fnFuncImplementation!(this.Name, args);

        public async ValueTask<Variant> InvokeAsync(Variant[] args, CancellationToken cancellation) =>
            this.IsAsync ? await _fnFuncImplementationAsync!(this.Name, args, cancellation) : Invoke(args);

        #endregion
    }
}
