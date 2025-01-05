using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Eval
{
    internal class JetBrainsLicenseReset
    {
        private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private readonly List<FileData> _keepFiles = new List<FileData>
        {
            new FileData()
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\keymaps",
                Name = @"Visual Studio copy.xml"
            },
            new FileData
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\resharper-host",
                Name = @"GlobalSettingsStorage.DotSettings"
            },
            new FileData()
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\options",
                Name = @"keymap.xml"
            },
            new FileData()
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\options",
                Name = @"recentSolutions.xml"
            },
            new FileData()
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\options",
                Name = @"recentSolutionsConfiguration.xml"
            },
            new FileData()
            {
                Path = $@"{AppDataFolder}\JetBrains\Rider2021.1\options",
                Name = @"trustedSolutions.xml"
            },
        };

        private readonly List<FolderData> _folderPosition = new List<FolderData>
        {
            new FolderData
            {
                Source = AppDataFolder,
                Folder = "JetBrains"
            },
            new FolderData
            {
                Source = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Folder = "JetBrains"
            },
        };

        private readonly List<RegistryData> _registryPosition = new List<RegistryData>
        {
            new RegistryData {RegistryKeyType = Registry.CurrentUser, Path = @"SOFTWARE\JetBrains"},
            new RegistryData
            {
                RegistryKeyType = Registry.Users,
                Path = @"S-1-5-21-3093079274-4027988934-4014703911-1001\SOFTWARE\JavaSoft\Prefs\jetbrains"
            },
        };


        private readonly List<RegistryData> _registrySpecialJavaFolderPosition = new List<RegistryData>
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
            var result = _directory.ClearFolders(_folderPosition);
            _directory.ReturnFilesToSourcePositions();
            return result;
        }

        public bool ClearTempFiles() => _directory.ClearTempFiles();

        public bool ClearRegistryMemory() => _registry.ClearRegistryMemory(_registryPosition);

        public bool ClearSpecialJavaMemory() => _registry.ClearRegistryMemory(_registrySpecialJavaFolderPosition);
    }
}