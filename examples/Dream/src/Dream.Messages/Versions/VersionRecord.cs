using System;
using Totem;

namespace Dream.Versions
{
    public class VersionRecord
    {
        public Id Id { get; set; } = null!;
        public Uri ZipUrl { get; set; } = null!;
        public ZipFileInfo? ZipFile { get; set; } = null!;
        public ZipContentInfo? ZipContent { get; set; } = null!;
        public VersionStatus Status { get; set; }

        public class ZipFileInfo
        {
            public FilePath Path { get; set; } = null!;
            public long ByteCount { get; set; }
        }

        public class ZipContentInfo
        {
            public int FileCount { get; set; }
            public long ByteCount { get; set; }
            public FilePath ExePath { get; set; } = null!;
        }
    }
}