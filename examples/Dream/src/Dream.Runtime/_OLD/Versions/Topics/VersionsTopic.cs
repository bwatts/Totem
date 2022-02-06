using System;
using System.Collections.Generic;
using System.IO;
using Totem;

namespace Dream.Versions.Topics;

public class VersionsTopic : Topic
{
    public static readonly Id InstanceId = (Id) "ada6077b-bc87-4bcd-b7b2-a95c7e84a2e1";

    public static Id Route(InstallVersion _) => InstanceId;

    readonly HashSet<string> _zipUrls = new(StringComparer.OrdinalIgnoreCase);

    public void Given(VersionInstalled e) =>
        _zipUrls.Add(e.ZipUrl.ToString());

    public void When(IHttpCommandContext<InstallVersion> context)
    {
        var zipUrl = context.Command.ZipUrl;
        var extension = Path.GetExtension(zipUrl.AbsolutePath);

        if(!StringComparer.OrdinalIgnoreCase.Equals(extension, ".zip"))
        {
            ThenError(RuntimeErrors.InvalidZipUrlExtension);
            return;
        }

        if(!_zipUrls.Contains(zipUrl.ToString()))
        {
            var versionId = Id.DeriveId(zipUrl);

            Then(new VersionInstalled(versionId, zipUrl));

            context.RespondCreated($"/versions/{versionId}");
        }
    }
}
