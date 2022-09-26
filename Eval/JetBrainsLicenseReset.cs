
#define ENABLE_LOG
#define ENABLE_DELETE_ACTION

using System;
using System.Collections.Generic;
using System.IO;
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
                    Console.WriteLine($"Error delete key:{pathToFolder} in path:{folderName} with exception: {e.Message}");
                    allPathsDelete = false;
                }
            }

            return allPathsDelete;
        }
        
        private bool DeleteFolder(string pathToFOlder, string folder)
        {
            var path = Path.Combine(pathToFOlder, folder);
            if (Directory.Exists(path) == false) 
                return false;
#if ENABLE_LOG
            Console.WriteLine($"[Folder]Delete key path:{path}");
#endif
            
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
                    Console.WriteLine($"Error delete key:{position.Item1} in path:{position.Item2} with exception: {e.Message}");
                    allPathsDelete = false;
                }
            }

            return allPathsDelete;
        }
        
        private void DeleteRegistryKey(RegistryKeyType registryKeyType, string path)
        {
            using (var key = GetRegistryKeyByType(registryKeyType))
            {
#if ENABLE_LOG
                Console.WriteLine($"[Registry]Delete key path:{path}");
#endif
                
#if ENABLE_DELETE_ACTION
                key.DeleteSubKeyTree(path);
#endif
            }
        }

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
    }
}