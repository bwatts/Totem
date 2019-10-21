using System.Threading.Tasks;
using Acme.ProductImport;
using Acme.ProductImport.Queries;
using Microsoft.AspNetCore.Mvc;
using Totem.Timeline.Mvc;

namespace Totem.Sample.Api.Controllers
{
  [ApiController]
  public class ImportsController : ControllerBase
  {
    [HttpGet("/api/imports")]
    public Task<IActionResult> GetStatus([FromServices] IQueryServer queries)
    {
      return queries.Get<ImportStatus>();
    }

    [HttpPost("/api/imports/start")]
    public Task<IActionResult> StartImport([FromServices] ICommandServer commands)
    {
      return commands.Execute(
        new StartImport("foo"),
        When<ImportStarted>.ThenOk,
        When<ImportAlreadyStarted>.ThenConflict);
    }

    [HttpPost("/api/imports/finish")]
    public Task<IActionResult> FinishImport([FromServices] ICommandServer commands)
    {
      return commands.Execute(
        new FinishImport("foo"),
        When<ImportFinished>.ThenOk,
        When<ImportNotStarted>.ThenConflict);
    }
  }
}
