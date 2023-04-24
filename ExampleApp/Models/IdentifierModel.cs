using XExpressions.VariantType;

namespace ExampleApp.Models
{
    public class IdentifierModel
    {
        public string Name { get; set; } = String.Empty;

        public string Value { get; set; } = String.Empty;

        public VariantKind ValueKind { get; set; } = VariantKind.String;
    }
}
