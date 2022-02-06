using System;
using System.Threading;
using System.Threading.Tasks;
using Dream.Versions;
using Totem;

namespace DreamUI.Installations.Topics;

public class InstallationTopic : Topic
{
    public static readonly Id TimelineId = (Id) "65d97717-460e-45db-8a21-b5cc7cbf14e3";
    public static readonly Uri ZipUrl = new("https://github.com/EventStore/Downloads/raw/master/win/EventStore-OSS-Windows-2019-v20.10.2.zip");

    bool _started;

    public void Given(InstallationStarted _) =>
        _started = true;

    public void Given(InstallVersionFailed _) =>
        _started = false;

    public void When(StartInstallation _)
    {
        if(!_started)
        {
            Then(new InstallationStarted(Id.NewId()));
        }
    }

    public async Task When(SendInstallVersion command, ITotemHttpClient client, CancellationToken cancellationToken)
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
