using Totem.Core;
using Totem.Map;
using Totem.Topics;

namespace Totem;



// WorkflowServiceTests
// ReportServiceTests



public abstract class TopicServiceTests<TTopic> where TTopic : ITopic
{
    //readonly RuntimeMap _map = new(typeof(TTopic));
    //TTopic? _topic;

    //protected TTopic InitTopic(TTopic topic)
    //{
    //    _topic = topic;

    //    if(topic.Id is null)
    //    {
    //        topic.Id = Id.NewId();
    //    }

    //    return topic;
    //}

    //protected static Id CallRoute(ICommandMessage command)
    //{
    //    throw new NotImplementedException();
    //}

    //protected void CallWhen(ICommandMessage command)
    //{
    //    throw new NotImplementedException();
    //}

    //protected T ExpectEvent<T>(Func<T, bool> firstWhere) where T : IEvent
    //{
    //    var e = GetTopic().NewEvents.OfType<T>().Where(firstWhere).FirstOrDefault();

    //    if(e is null)
    //        throw new ExpectException($"Expected event of type {typeof(T)} matching {firstWhere}");

    //    return e;
    //}

    //protected void ExpectError(ErrorInfo error)
    //{
    //    if(error is null)
    //        throw new ArgumentNullException(nameof(error));

    //    if(!GetTopic().Errors.Contains(error))
    //        throw new ExpectException($"Expected error {error}");
    //}

    //TTopic GetTopic() =>
    //    _topic ?? throw new InvalidOperationException($"Topic is not initialized; call {nameof(InitTopic)} first");
}
