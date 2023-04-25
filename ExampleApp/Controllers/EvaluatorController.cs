using ExampleApp.Models;
using Microsoft.AspNetCore.Mvc;
using XExpressions.SVG;
using XExpressions.VariantType;

namespace XExpressions.RestApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class EvaluatorController : ControllerBase
    {
        #region Construction

        public EvaluatorController(ILogger<EvaluatorController> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Fields

        private readonly ILogger<EvaluatorController> _logger;

        #endregion

        #region Methods

        [HttpPost("evaluate")]
        public ActionResult Post([FromBody] EvaluateRequestModel requestModel)
        {
            try
            {
                Dictionary<string, Variant> identifiers = new Dictionary<string, Variant>();

                XExpressionsSettings settings = new XExpressionsSettings();
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
                        settings.AddIdentifier(ident.Name, name => identValue);
                }

                Evaluator eval = new Evaluator(requestModel.Expression, settings);

                Variant result = eval.Evaluate();

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

        [HttpGet("svg")]
        public ActionResult GetSvg(string expression)
        {
            Evaluator eval = new Evaluator(expression);

            string svg = eval.RootNode.CreateSvg();

            return Content(svg, "image/svg+xml");
        }

        #endregion
    }
}
