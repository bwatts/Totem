using System;
using System.Threading;
using System.Threading.Tasks;
using Dream.Versions;
using Totem;

namespace DreamUI.Installations.Timelines
{
    public class InstallationTimeline : Timeline
    {
        public static readonly Id TimelineId = (Id) "65d97717-460e-45db-8a21-b5cc7cbf14e3";
        public static readonly Uri ZipUrl = new("https://github.com/EventStore/Downloads/raw/master/win/EventStore-OSS-Windows-2019-v20.10.2.zip");

        bool _installing;

        public InstallationTimeline(Id id) : base(id)
        {
            Given<InstallationStarted>(e => _installing = true);
            Given<InstallVersionFailed>(e => _installing = false);
        }

        public void When(StartInstallation _)
        {
            if(!_installing)
            {
                Then(new InstallationStarted(Id.NewId()));
            }
        }

        public async Task WhenAsync(SendInstallVersion command, ITotemClient client, CancellationToken cancellationToken)
        {
            try
            {
                await client.SendAsync(new InstallVersion { ZipUrl = ZipUrl }, cancellationToken);

                Then(new InstallVersionSent(command.InstallationId));
            }
            catch(Exception exception)
            {
                Then(new InstallVersionFailed(command.InstallationId, exception.ToString()));
            }
        }
    }
}