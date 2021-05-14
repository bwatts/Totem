using System;

namespace Totem.Files
{
    public class LocalFileStorageSettings : ILocalFileStorageSettings
    {
        public LocalFileStorageSettings(string baseDirectory)
        {
            if(string.IsNullOrWhiteSpace(baseDirectory))
                throw new ArgumentOutOfRangeException(nameof(baseDirectory));

            BaseDirectory = baseDirectory;
        }

        public string BaseDirectory { get; }
    }
}