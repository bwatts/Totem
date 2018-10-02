using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Totem.Timeline.Mvc
{
  /// <summary>
  /// Describes a server responding to GET requests to queries
  /// </summary>
  public interface IQueryServer
  {
    Task<IActionResult> Get(Type type, Id id);
  }

  /// <summary>
  /// Extends <see cref="IQueryServer"/> with methods to serve GET requests
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class QueryServerExtensions
  {
    public static Task<IActionResult> Get(this IQueryServer server, Type type) =>
      server.Get(type, Id.Unassigned);

    public static Task<IActionResult> Get(this IQueryServer server, Type type, string id) =>
      server.Get(type, Id.From(id));

    public static Task<IActionResult> Get<T>(this IQueryServer server) where T : Query =>
      server.Get(typeof(T), Id.Unassigned);

    public static Task<IActionResult> Get<T>(this IQueryServer server, string id) where T : Query =>
      server.Get(typeof(T), Id.From(id));

    public static Task<IActionResult> Get<T>(this IQueryServer server, Id id) where T : Query =>
      server.Get(typeof(T), id);
  }
}