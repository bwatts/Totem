namespace Totem.Workflows
{
    public class WorkflowSettings : IWorkflowSettings
    {
        public int SubscriptionCapacity { get; set; } = 500;
    }
}