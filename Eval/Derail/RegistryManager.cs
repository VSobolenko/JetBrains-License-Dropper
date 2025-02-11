using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Eval.Derail
{
    public class RegistryManager
    {
        private readonly Action<string> _log;

        public RegistryManager(Action<string> log) => _log = log;

        public bool ClearRegistryMemory(IEnumerable<RegistryData> positions)
        {
            var allPathsDelete = true;
            foreach (var position in positions)
            {
                try
                {
                    DeleteRegistryKey(position.RegistryKeyType, position.Path);
                }
                catch (Exception e)
                {
                    _log($"[Registry][Error] Delete key:{position.RegistryKeyType} in path {position.Path} " +
                         $"with exception: {e.Message}");
                    allPathsDelete = false;
                }
            }

            return allPathsDelete;
        }

        private void DeleteRegistryKey(RegistryKey registryKeyType, string path)
        {
#if !FEATURE_DELETION_PROTECTION
            using (registryKeyType)
                registryKeyType.DeleteSubKeyTree(path);
#endif

            _log($"[Registry][Success] Delete key path:{path}");
        }
    }
}