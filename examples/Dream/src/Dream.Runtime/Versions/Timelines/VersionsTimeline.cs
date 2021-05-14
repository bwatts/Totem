using System;
using System.Collections.Generic;
using System.IO;
using Totem;

namespace Dream.Versions.Timelines
{
    public class VersionsTimeline : Timeline
    {
        public static readonly Id TimelineId = (Id) "ada6077b-bc87-4bcd-b7b2-a95c7e84a2e1";

        readonly HashSet<string> _zipUrls = new(StringComparer.OrdinalIgnoreCase);

        public VersionsTimeline(Id id) : base(id)
        {
            Given<VersionInstalled>(e => _zipUrls.Add(e.ZipUrl.ToString()));
        }

        public void When(InstallVersion command)
        {
            var extension = Path.GetExtension(command.ZipUrl.AbsolutePath);

            if(!StringComparer.OrdinalIgnoreCase.Equals(extension, ".zip"))
            {
                ThenError(RuntimeErrors.InvalidZipUrlExtension);
                return;
            }

            if(!_zipUrls.Contains(command.ZipUrl.ToString()))
            {
                Then(new VersionInstalled(Id.DeriveId(command.ZipUrl), command.ZipUrl));
            }
        }
    }
}