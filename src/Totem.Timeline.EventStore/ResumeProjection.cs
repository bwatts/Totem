using System.IO;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Totem.Runtime;
using Totem.Timeline.Area;

namespace Totem.Timeline.EventStore
{
  /// <summary>
  /// The projection installed to track the set of flows to resume
  /// </summary>
  public sealed class ResumeProjection : Notion, IResumeProjection
  {
    readonly AreaMap _area;
    readonly ProjectionsManager _manager;
    readonly UserCredentials _credentials;

    public ResumeProjection(AreaMap area, ProjectionsManager manager, UserCredentials credentials)
    {
      _area = area;
      _manager = manager;
      _credentials = credentials;
    }

    public async Task Synchronize()
    {
      var name = $"{_area}-resume";

      if(await NotFound(name))
      {
        await Create(name);
      }
    }

    async Task<bool> NotFound(string name)
    {
      try
      {
        await _manager.GetStatusAsync(name, _credentials);

        return false;
      }
      catch(ProjectionCommandFailedException error)
      {
        if(error.HttpStatusCode != 404)
        {
          throw;
        }

        return true;
      }
    }

    async Task Create(string name)
    {
      await _manager.CreateContinuousAsync(name, await ReadScript(), _credentials);

      Log.Debug("[timeline] Created projection {Name}", name);
    }

    async Task<string> ReadScript()
    {
      var type = GetType();

      using(var resource = type.Assembly.GetManifestResourceStream(type, "resume-projection.js"))
      using(var reader = new StreamReader(resource))
      {
        var content = await reader.ReadToEndAsync();

        return new StringBuilder("let area = ")
          .Append('"')
          .Append(_area)
          .Append("\";")
          .AppendLine()
          .AppendLine()
          .Append(content)
          .ToString();
      }
    }

    public static IResumeProjection Assumed() => new AssumedProjection();

    class AssumedProjection : Notion, IResumeProjection
    {
      public Task Synchronize()
      {
        Log.Debug("[timeline] Assuming resume projection is installed");

        return Task.CompletedTask;
      }
    }
  }
}