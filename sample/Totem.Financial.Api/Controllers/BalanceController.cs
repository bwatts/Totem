using System.Threading.Tasks;
using Acme.ProductImport;
using Acme.ProductImport.Queries;
using Microsoft.AspNetCore.Mvc;
using Totem.Timeline.Mvc;

namespace Totem.Sample.Api.Controllers
{
  [ApiController]
  public class BalanceController : ControllerBase
  {
    [HttpGet("/api/balance")]
    public Task<IActionResult> GetStatus([FromServices] IQueryServer queries)
    {
      return queries.Get<BalanceQuery>();
    }

    [HttpPost("/api/balance")]
    public Task<IActionResult> SetBalance([FromServices] ICommandServer commands, SetBalanceRequest request)
    {
      return commands.Execute(
        new SetBalance(request.Balance),
        When<BalanceSet>.ThenOk);
    }
  }

  public class SetBalanceRequest
  {
    public int Balance { get; set; }
  }
}
