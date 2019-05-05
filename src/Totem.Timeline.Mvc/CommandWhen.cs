using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// An event that responds to a pending MVC command
  /// </summary>
  public class CommandWhen : ICommandWhen<IActionResult>
  {
    readonly Func<Event, IActionResult> _respond;

    public CommandWhen(Type eventType, Func<Event, IActionResult> respond)
    {
      EventType = eventType;
      _respond = respond;
    }

    public Type EventType { get; }

    public Task<IActionResult> Respond(Event e) =>
      Task.FromResult(_respond(e));
  }
}