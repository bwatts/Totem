using System;

namespace Totem.Files;

public class DiskStorageSettings : IDiskStorageSettings
{
    public DiskStorageSettings(string baseDirectory)
    {
        if(string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentOutOfRangeException(nameof(baseDirectory));

        BaseDirectory = baseDirectory;
    }

    public string BaseDirectory { get; }
}
