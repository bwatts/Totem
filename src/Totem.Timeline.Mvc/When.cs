using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Declares an HTTP response to the specific event type
  /// </summary>
  /// <typeparam name="TEvent">The type of event to which to respond</typeparam>
  public static class When<TEvent> where TEvent : Event
  {
    public static CommandWhen Then(Func<TEvent, IActionResult> respond) =>
      new CommandWhen(typeof(TEvent), e => respond((TEvent) e));

    public static CommandWhen Then(int statusCode) =>
      Then(e => new StatusCodeResult(statusCode));

    public static CommandWhen ThenOk =>
      Then(e => new OkResult());

    public static CommandWhen ThenCreated(Func<TEvent, string> getLocation, Func<TEvent, object> getValue) =>
      Then(e => new CreatedResult(getLocation(e), getValue(e)));

    public static CommandWhen ThenCreated(Func<TEvent, string> getLocation) =>
      Then(e => new CreatedResult(getLocation(e), null));

    public static CommandWhen ThenCreated(Func<TEvent, Uri> getLocation, Func<TEvent, object> getValue) =>
      Then(e => new CreatedResult(getLocation(e), getValue(e)));

    public static CommandWhen ThenCreated(Func<TEvent, Uri> getLocation) =>
      Then(e => new CreatedResult(getLocation(e), null));

    public static CommandWhen ThenBadRequest =>
      Then(e => new BadRequestResult());

    public static CommandWhen ThenNotFound =>
      Then(e => new NotFoundResult());

    public static CommandWhen ThenConflict =>
      Then(StatusCodes.Status409Conflict);
  }
}