namespace ExampleApp.Models
{
    public class EvaluateRequestModel
    {
        public string Expression { get; set; } = String.Empty;

        public IEnumerable<IdentifierModel> Identifiers { get; set; } = Enumerable.Empty<IdentifierModel>();
    }
}
