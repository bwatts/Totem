using System.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Totem.Timeline.Client;

namespace Totem.Timeline.SignalR.Hosting
{
  /// <summary>
  /// Extends <see cref="ISignalRServerBuilder"/> to declare timeline SignalR extensions
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineSignalRServerBuilderExtensions
  {
    public static ISignalRServerBuilder AddQueryNotifications(this ISignalRServerBuilder signalR)
    {
      signalR.Services.AddScoped<QueryHub>();

      signalR.Services.AddSingleton<IQueryNotifier, QueryNotifier>();

      return signalR;
    }
  }
}