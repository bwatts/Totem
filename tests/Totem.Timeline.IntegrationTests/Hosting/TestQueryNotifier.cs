using System.Collections.Generic;
using System.Threading.Tasks;
using Totem.Timeline.Client;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Captures query changed notifications for <see cref="TestTimeline"/>
  /// </summary>
  internal sealed class TestQueryNotifier : IQueryNotifier
  {
    readonly Dictionary<QueryETag, TaskSource> _tasksByETag = new Dictionary<QueryETag, TaskSource>();

    public Task NotifyChanged(QueryETag etag, IEnumerable<Id> connectionIds)
    {
      RemoveOrStartTask(etag).TrySetResult();

      return Task.CompletedTask;
    }

    internal Task WaitUntilChanged(Id connectionId, QueryETag etag) =>
      RemoveOrStartTask(etag).Task;

    TaskSource RemoveOrStartTask(QueryETag etag)
    {
      lock(_tasksByETag)
      {
        if(!_tasksByETag.Remove(etag, out var task))
        {
          task = new TaskSource();

          _tasksByETag[etag] = task;
        }

        return task;
      }
    }
  }
}