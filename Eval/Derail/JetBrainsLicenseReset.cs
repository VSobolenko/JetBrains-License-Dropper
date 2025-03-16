using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Eval.Derail
{
    internal class JetBrainsLicenseReset
    {
        private static readonly string AppDataFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static readonly string LocalAppDataFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        private readonly List<FileSystemEntry> _keepFiles = new List<FileSystemEntry>
        {
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\keymaps\Visual Studio copy.xml"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\resharper-host\GlobalSettingsStorage.DotSettings"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\options\keymap.xml"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\options\recentSolutions.xml"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\options\recentSolutionsConfiguration.xml"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\options\trustedSolutions.xml"),
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\tools\External Tools.xml"),
        };

        private readonly List<FileSystemEntry> _keepFolders = new List<FileSystemEntry>
        {
            new($@"{AppDataFolder}\JetBrains\Rider2021.1\filetypes"),
        };

        private readonly List<FileSystemEntry> _deletableFolderPosition = new List<FileSystemEntry>
        {
            new($@"{AppDataFolder}\JetBrains"),
            new($@"{LocalAppDataFolder}\JetBrains"),
        };

        private readonly List<RegistryData> _deletableRegistryPosition = new List<RegistryData>
        {
            new RegistryData {RegistryKeyType = Registry.CurrentUser, Path = @"SOFTWARE\JetBrains"},
            new RegistryData
            {
                RegistryKeyType = Registry.Users,
                Path = @"S-1-5-21-3093079274-4027988934-4014703911-1001\SOFTWARE\JavaSoft\Prefs\jetbrains"
            },
        };


        private readonly List<RegistryData> _deletableRegistrySpecialJavaFolderPosition = new List<RegistryData>
        {
            new RegistryData
            {
                RegistryKeyType = Registry.CurrentUser,
                Path = @"SOFTWARE\JavaSoft\Prefs\jetbrains"
            },
            new RegistryData
            {
                RegistryKeyType = Registry.Users,
                Path = @"S-1-5-21-3093079274-4027988934-4014703911-1001\SOFTWARE\JavaSoft\Prefs\jetbrains"
            },
        };

        private readonly DirectoryManager _directory;
        private readonly RegistryManager _registry;

        public JetBrainsLicenseReset(Action<string> log)
        {
            _directory = new DirectoryManager(log);
            _registry = new RegistryManager(log);
        }

        public bool ClearFolderMemoryWithSavingSettings()
        {
            foreach (var fileData in _keepFiles)
                _directory.KeepFile(fileData.Path, fileData.Name);
            foreach (var folderData in _keepFolders)
                _directory.KeepFolder(folderData.Source, folderData.Name);

            var result = _directory.ClearFolders(_deletableFolderPosition);

            _directory.ReturnFoldersToSourcePositions();
            _directory.ReturnFilesToSourcePositions();
            return result;
        }

        public bool ClearTempFiles() => _directory.ClearTempFiles();

        public bool ClearRegistryMemory() => _registry.ClearRegistryMemory(_deletableRegistryPosition);

        public bool ClearSpecialJavaMemory() =>
            _registry.ClearRegistryMemory(_deletableRegistrySpecialJavaFolderPosition);
    }
}