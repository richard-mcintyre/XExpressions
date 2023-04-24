using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    public class XExpressionsSettings
    {
        public static readonly XExpressionsSettings Default = CreateDefault();

        private const string DefaultFunctionName_Min = "min";
        private const string DefaultFunctionName_Max = "max";

        #region Construction

        static XExpressionsSettings()
        {
            // Add the default functions
            _defaultFunctions.Add(DefaultFunctionName_Min, 
                new FunctionDef(DefaultFunctionName_Min, 2, (name, args) => Math.Min((decimal)args[0], (decimal)args[1])));

            _defaultFunctions.Add(DefaultFunctionName_Max, 
                new FunctionDef(DefaultFunctionName_Max, 2, (name, args) => Math.Max((decimal)args[0], (decimal)args[1])));
        }

        #endregion

        #region Fields

        private static readonly Dictionary<string, FunctionDef> _defaultFunctions = 
            new Dictionary<string, FunctionDef>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, FunctionDef> _functions = new Dictionary<string, FunctionDef>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, IdentifierDef> _identifiers = new Dictionary<string, IdentifierDef>(StringComparer.InvariantCultureIgnoreCase);

        #endregion

        #region Properties

        /// <summary>
        /// Determines if the default functions (such as min, max etc...) should be available to the expression
        /// </summary>
        public bool IncludeDefaultFunctions { get; set; } = true;

        #endregion

        #region Methods

        /// <summary>
        /// Adds a custom function to the expression language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterCount"></param>
        /// <param name="fnFuncImplementation"></param>
        public void AddFunction(string name, int parameterCount, Func<string, Variant[], Variant> fnFuncImplementation) =>
            _functions.Add(name, new FunctionDef(name, parameterCount, fnFuncImplementation));

        /// <summary>
        /// Adds a custom function to the expression language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameterCount"></param>
        /// <param name="fnFuncImplementationAsync"></param>
        public void AddFunction(string name, int parameterCount, Func<string, Variant[], CancellationToken, ValueTask<Variant>> fnFuncImplementationAsync) =>
            _functions.Add(name, new FunctionDef(name, parameterCount, fnFuncImplementationAsync));

        internal bool TryGetFunction(string name, [NotNullWhen(true)] out FunctionDef? function)
        {
            if (this.IncludeDefaultFunctions && _defaultFunctions.TryGetValue(name, out function))
                return true;

            return _functions.TryGetValue(name, out function);
        }

        /// <summary>
        /// Adds an identifier (constant/variable) to the expression language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fnGetValue"></param>
        public void AddIdentifier(string name, Func<string, Variant?> fnGetValue) =>
            _identifiers.Add(name, new IdentifierDef(name, fnGetValue));

        /// <summary>
        /// Adds an identifier (constant/variable) to the expression language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fnGetValueAsync"></param>
        public void AddIdentifier(string name, Func<string, CancellationToken, ValueTask<Variant?>> fnGetValueAsync) =>
            _identifiers.Add(name, new IdentifierDef(name, fnGetValueAsync));

        internal bool TryGetIdentifier(string name, [NotNullWhen(true)] out IdentifierDef? identifier) =>
            _identifiers.TryGetValue(name, out identifier);

        private static XExpressionsSettings CreateDefault() => new XExpressionsSettings();

        #endregion
    }
}
