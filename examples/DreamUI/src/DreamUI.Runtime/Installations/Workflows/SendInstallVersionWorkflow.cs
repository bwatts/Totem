using Totem;

namespace DreamUI.Installations.Workflows
{
    public class SendInstallVersionWorkflow : Workflow
    {
        public static readonly Id WorkflowId = (Id) "ef79236d-27db-4f63-aa60-3d674a6e3730";

        public static Id Route(InstallationStarted _) => WorkflowId;

        public void When(InstallationStarted e) =>
            ThenEnqueue(new SendInstallVersion { InstallationId = e.InstallationId });
    }
}