using System.IO;
using Microsoft.Win32;

namespace Eval
{
    internal struct FileData
    {
        public string Path;
        public string Name;
    }

    public struct FolderData
    {
        public string Source;
        public string Folder;

        public string GetPath() => Path.Combine(Source, Folder);
    }

    public struct RegistryData
    {
        public RegistryKey RegistryKeyType;
        public string Path;
    }
}