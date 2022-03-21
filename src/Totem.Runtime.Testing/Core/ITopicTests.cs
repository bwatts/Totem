using Totem.Map;
using Totem.Topics;

namespace Totem.Core;

internal interface ITopicTests
{
    RuntimeMap RuntimeMap { get; }
    ITopic Topic { get; }

    void OnCallingWhen();
    void OnWhenCalled();
    void OnExpectingCommand();
}
