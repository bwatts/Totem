using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Describes a server responding to POST, PUT, DELETE, PATCH, and other write requests
  /// </summary>
  public interface ICommandServer
  {
    Task<IActionResult> Execute(Command command, IEnumerable<CommandWhen> whens);

    Task<IActionResult> Execute(Command command, params CommandWhen[] whens);
  }
}