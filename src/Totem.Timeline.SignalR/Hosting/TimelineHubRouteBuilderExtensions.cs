using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

namespace Totem.Timeline.SignalR.Hosting
{
  /// <summary>
  /// Extends <see cref="HubRouteBuilder"/> to declare timeline hubs
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineHubRouteBuilderExtensions
  {
    public static void MapQueryHub(this HubRouteBuilder routes, Action<HttpConnectionDispatcherOptions> configure, string path = "/hubs/query") =>
      routes.MapHub<QueryHub>(path, configure);

    public static void MapQueryHub(this HubRouteBuilder routes, string path = "/hubs/query") =>
      routes.MapHub<QueryHub>(path);
  }
}