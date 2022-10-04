
#define ENABLE_LOG
#define ENABLE_DELETE_ACTION

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace Eval
{
    internal enum RegistryKeyType
    {
        CurrentUser,
        Users,
    }
    
    internal class JetBrainsLicenseReset
    {
        private readonly List<string> _tempFolders = new List<string>
        {
            Path.GetTempPath(),
        };
        
        private readonly List<Tuple<string, string>> _folderPosition = new List<Tuple<string, string>>
        {
            new Tuple<string, string>(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JetBrains"),
            new Tuple<string, string>(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JetBrains"),
        };

        private readonly List<Tuple<RegistryKeyType, string>> _registryPosition = new List<Tuple<RegistryKeyType, string>>
        {
            new Tuple<RegistryKeyType, string>(RegistryKeyType.CurrentUser, @"SOFTWARE\JetBrains"),
            new Tuple<RegistryKeyType, string>(RegistryKeyType.Users, @"S-1-5-21-3093079274-4027988934-4014703911-1001\SOFTWARE\JavaSoft\Prefs\jetbrains"),
        };
        
        private readonly List<Tuple<RegistryKeyType, string>> _registrySpecialJavaPosition = new List<Tuple<RegistryKeyType, string>>
        {
            new Tuple<RegistryKeyType, string>(RegistryKeyType.CurrentUser, @"SOFTWARE\JavaSoft\Prefs\jetbrains"),
            new Tuple<RegistryKeyType, string>(RegistryKeyType.Users, @"S-1-5-21-3093079274-4027988934-4014703911-1001\SOFTWARE\JavaSoft\Prefs\jetbrains"),
        };

        private readonly Action<string> _log;

        public JetBrainsLicenseReset(Action<string> log) => _log = log;
        
        #region Folder

        public bool ClearFolderMemory() => ClearFolderMemory(_folderPosition);

        private bool ClearFolderMemory(IEnumerable<Tuple<string, string>> positions)
        {
            var allPathsDelete = true;
            foreach (var (pathToFolder, folderName) in positions)
            {
                try
                {
                    allPathsDelete = DeleteFolder(pathToFolder, folderName) && allPathsDelete;
                }
                catch (Exception e)
                {
                    Log($"[Folder][Error] Delete key:{pathToFolder} in path:{folderName} with exception: {e.Message}");
                    allPathsDelete = false;
                }
            }

            return allPathsDelete;
        }

        public bool ClearTempFiles() => ClearFolderInsides(_tempFolders);
        
        private bool ClearFolderInsides(IEnumerable<string> positions)
        {
            foreach (var position in positions)
            {
                var directoryInfo = new DirectoryInfo(position);

                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    try
                    {
                        var filePath = Path.Combine(file.Name);
#if ENABLE_DELETE_ACTION
                        file.Delete(); 
#endif
                        Log($"[Folder][Success] Delete file: {filePath}");
                    }
                    catch (Exception e)
                    {
                        Log($"[Folder][Error] Delete file: {e.Message}");
                    } 
                }
                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    try
                    {
                        var directoryPath = directory.FullName;
#if ENABLE_DELETE_ACTION
                        directory.Delete(true);
#endif
                        Log($"[Folder][Success] Delete file: {directoryPath}");
                    }
                    catch (Exception e)
                    {
                        Log($"[Folder][Error] Delete error: {e.Message}");
                    }
                }
            }
            return true;
        }
        
        private bool DeleteFolder(string pathToFOlder, string folder)
        {
            var path = Path.Combine(pathToFOlder, folder);
            if (Directory.Exists(path) == false) 
                return false;
            Log($"[Folder][Success] Delete key path:{path}");
            
#if ENABLE_DELETE_ACTION
            Directory.Delete(path);
#endif
            return true;
        }

        #endregion

        #region Registry

        public bool ClearRegistryMemory() => ClearRegistryMemory(_registryPosition);
        
        public bool ClearSpecialJavaMemory() => ClearRegistryMemory(_registrySpecialJavaPosition);

        private bool ClearRegistryMemory(List<Tuple<RegistryKeyType, string>> positions)
        {
            var allPathsDelete = true;
            foreach (var position in positions)
            {
                try
                {
                    DeleteRegistryKey(position.Item1, position.Item2);
                }
                catch (Exception e)
                {
                    Log($"[Registry][Error] Delete key:{position.Item1} in path:{position.Item2} with exception: {e.Message}");
                    allPathsDelete = false;
                }
            }

            return allPathsDelete;
        }
        
        private void DeleteRegistryKey(RegistryKeyType registryKeyType, string path)
        {
            using (var key = GetRegistryKeyByType(registryKeyType))
            {
                Log($"[Registry][Success] Delete key path:{path}");
                
#if ENABLE_DELETE_ACTION
                key.DeleteSubKeyTree(path);
#endif
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RegistryKey GetRegistryKeyByType(RegistryKeyType registryKeyType)
        {
            switch (registryKeyType)
            {
                case RegistryKeyType.CurrentUser:
                    return Registry.CurrentUser;
                case RegistryKeyType.Users:
                    return Registry.Users;
                default:
                    throw new ArgumentOutOfRangeException(nameof(registryKeyType), registryKeyType, "Unsupported registry key Used");
            }
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Log(string message)
        {
#if ENABLE_LOG
            _log?.Invoke(message);
#endif
        }
    }
}