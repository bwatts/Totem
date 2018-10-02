using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// A server for POST, PUT, PATCH, DELETE, and other write requests
  /// </summary>
  public sealed class CommandServer : ICommandServer
  {
    readonly ICommandHost _host;

    public CommandServer(ICommandHost host)
    {
      _host = host;
    }

    public Task<IActionResult> Execute(Command command, IEnumerable<CommandWhen> whens) =>
      _host.Execute(command, whens);

    public Task<IActionResult> Execute(Command command, params CommandWhen[] whens) =>
      _host.Execute(command, whens);
  }
}