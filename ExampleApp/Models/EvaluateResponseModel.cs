using XExpressions.VariantType;

namespace ExampleApp.Models
{
    public class EvaluateResponseModel
    {
        public EvaluateResponseModel(Variant value)
        {
            this.Kind = value.Kind;

            switch (value.Kind)
            {
                case VariantKind.Boolean:
                    this.Value = ((bool)value) ? Boolean.TrueString : Boolean.FalseString;
                    break;
                case VariantKind.Decimal:
                    this.Value = Convert.ToString((decimal)value);
                    break;
                case VariantKind.String:
                    this.Value = (string)value;
                    break;

                default:
                    this.Value = String.Empty;
                    break;
            }
        }

        public VariantKind Kind { get; }

        public string Value { get; }
    }

}
