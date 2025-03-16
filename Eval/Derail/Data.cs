using Microsoft.Win32;

namespace Eval.Derail
{
    internal readonly struct FileSystemEntry
    {
        public string Path { get; }
        public string Source => System.IO.Path.GetDirectoryName(Path);
        public string Name => System.IO.Path.GetFileName(Path);

        public FileSystemEntry(string path)
        {
            Path = path;
        }
    }

    public struct RegistryData
    {
        public RegistryKey RegistryKeyType;
        public string Path;
    }
}