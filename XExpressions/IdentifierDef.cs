using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExpressions.VariantType;

namespace XExpressions
{
    internal record IdentifierDef(string Name, Func<string, Variant?> fnGetValue);

}
