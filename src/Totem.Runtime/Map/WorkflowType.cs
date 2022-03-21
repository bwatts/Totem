using Totem.Workflows;

namespace Totem.Map;

public class WorkflowType : ObserverType
{
    internal WorkflowType(Type declaredType, ObserverConstructor constructor) : base(declaredType, constructor)
    { }

    internal new IWorkflow Create(Id id) =>
        (IWorkflow) base.Create(id);
}
