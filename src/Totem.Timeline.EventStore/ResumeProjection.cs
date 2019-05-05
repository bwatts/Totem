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
      if(await StreamNotFound())
      {
        await CreateStream();
      }
    }

    async Task<bool> StreamNotFound()
    {
      try
      {
        await _manager.GetStatusAsync(TimelineStreams.Resume, _credentials);

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

    async Task CreateStream()
    {
      await _manager.CreateContinuousAsync(TimelineStreams.Resume, await ReadScript(), _credentials);

      Log.Debug("[timeline] Created projection {Name}", TimelineStreams.Resume);
    }

    async Task<string> ReadScript()
    {
      var type = GetType();

      using(var resource = type.Assembly.GetManifestResourceStream(type, "resume-projection.js"))
      using(var reader = new StreamReader(resource))
      {
        return await reader.ReadToEndAsync();
      }
    }
  }
}