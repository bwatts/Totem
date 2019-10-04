using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Totem.Timeline.Client;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// An event that responds to a pending MVC command
  /// </summary>
  public sealed class CommandWhen : ICommandWhen<IActionResult>
  {
    readonly Type _eventType;
    readonly Func<Event, IActionResult> _respond;

    public CommandWhen(Type eventType, Func<Event, IActionResult> respond)
    {
      _eventType = eventType;
      _respond = respond;
    }

    public bool CanRespond(TimelinePoint point) =>
      point.Type.DeclaredType == _eventType;

    public Task<IActionResult> Respond(TimelinePoint point) =>
      Task.FromResult(_respond(point.Event));
  }
}