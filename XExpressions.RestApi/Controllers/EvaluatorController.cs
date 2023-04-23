using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using XExpressions.VariantType;

namespace XExpressions.RestApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class EvaluatorController : ControllerBase
    {
        public EvaluatorController(ILogger<EvaluatorController> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<EvaluatorController> _logger;

        [HttpGet("svg")]
        public ActionResult GetSvg(string expression)
        {
            Evaluator eval = new Evaluator(expression);

            string svg = eval.RootNode.CreateSvg();

            return Content(svg, "image/svg+xml");
        }

        [HttpPost("evaluate")]
        public ActionResult Post([FromBody] EvaluateRequestModel requestModel)
        {
            try
            {
                Dictionary<string, Variant> identifiers = new Dictionary<string, Variant>();

                foreach (IdentifierModel ident in requestModel.Identifiers)
                {
                    if (identifiers.ContainsKey(ident.Name))
                        continue;

                    Variant? identValue = null;

                    switch (ident.ValueKind)
                    {
                        case VariantKind.Boolean:
                            identValue = Convert.ToBoolean(ident.Value);
                            break;
                        case VariantKind.Decimal:
                            identValue = Convert.ToDecimal(ident.Value);
                            break;
                        case VariantKind.String:
                            identValue = ident.Value;
                            break;
                    }

                    if (identValue != null)
                        identifiers.Add(ident.Name, identValue.Value);
                }

                Evaluator eval = new Evaluator(requestModel.Expression);

                Variant result = eval.Evaluate(identName =>
                {
                    if (identifiers.TryGetValue(identName, out var identValue))
                        return identValue;

                    return null;
                });

                return Ok(new EvaluateResponseModel(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails()
                {
                    Title = ex.Message
                });
            }
        }

        public class EvaluateRequestModel
        {
            public string Expression { get; set; } = String.Empty;

         
            public IEnumerable<IdentifierModel> Identifiers { get; set; } = Enumerable.Empty<IdentifierModel>();
        }

        public class IdentifierModel
        {
            public string Name { get; set; } = String.Empty;

            public string Value { get; set; } = String.Empty;

            public VariantKind ValueKind { get; set; } = VariantKind.String;
        }

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
}