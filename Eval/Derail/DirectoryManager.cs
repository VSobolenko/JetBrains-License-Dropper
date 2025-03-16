using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eval.Derail
{
    internal class DirectoryManager
    {
        private readonly Action<string> _log;

        private readonly Dictionary<string, RootObjectTransfer> _keepFiles = new();

        private readonly Dictionary<string, RootObjectTransfer> _keepDirectories = new();

        public DirectoryManager(Action<string> log) => _log = log;

        public bool ClearFolders(IEnumerable<FileSystemEntry> positions)
        {
            var allPathsDelete = true;
            foreach (var position in positions)
                allPathsDelete = DeleteFolder(position.Path);

            return allPathsDelete;
        }

        private bool DeleteFolder(string path2Folder)
        {
            try
            {
                DeleteDirectory(path2Folder);
            }
            catch (Exception e)
            {
                _log($"[Folder][Error] Delete folder:{path2Folder} with exception: {e.Message}");
                return false;
            }

            return true;
        }

        private void DeleteDirectory(string path2Folder)
        {
            if (Directory.Exists(path2Folder) == false) return;

            _log($"[Folder][Success] Delete key path: {path2Folder}");
#if !FEATURE_DELETION_PROTECTION
            Directory.Delete(path2Folder, true);
#endif
        }

        public void KeepFile(string folderPath, string fileName)
        {
            var file = new RootObjectTransfer
            {
                SourceDirectory = folderPath,
                SourcePosition = Path.Combine(folderPath, fileName),
                TempPosition = Path.Combine(Path.GetTempPath(), fileName),
            };
            if (File.Exists(file.SourcePosition) == false)
            {
                _log($"[Folder][Error] File Not Found: {file.SourcePosition}");
                return;
            }

            _keepFiles[file.SourcePosition] = file;

#if !FEATURE_DELETION_PROTECTION
            File.Move(file.SourcePosition, file.TempPosition);
#endif
        }

        public void KeepFolder(string solderSource, string folderName)
        {
            var directory = new RootObjectTransfer
            {
                SourceDirectory = solderSource,
                SourcePosition = Path.Combine(solderSource, folderName),
                TempPosition = Path.Combine(Path.GetTempPath(), folderName),
            };

            if (Directory.Exists(directory.SourcePosition) == false)
            {
                _log($"[Folder][Error] Folder Not Found: {directory.SourcePosition}");
                return;
            }

            _keepDirectories[directory.SourcePosition] = directory;


#if !FEATURE_DELETION_PROTECTION
            Directory.Move(directory.SourcePosition, directory.TempPosition);
#endif
        }

        public IEnumerable<RootObjectTransfer> ReturnFilesToSourcePositions()
        {
            foreach (var filePosition in _keepFiles)
            {
                var folder = filePosition.Value.SourceDirectory;
                var fileName = Path.GetFileName(filePosition.Key);

                _log($"[Folder][Success] Save file: \"{fileName}\" in path: {filePosition.Value.SourcePosition}");
#if !FEATURE_DELETION_PROTECTION
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);

                File.Move(filePosition.Value.TempPosition, filePosition.Value.SourcePosition);
#endif
            }

            var transfers = _keepFiles.Select(x => x.Value);
            _keepFiles.Clear();
            return transfers;
        }

        public void ReturnFoldersToSourcePositions()
        {
            foreach (var folderPosition in _keepDirectories)
            {
                var folderName = Path.GetFileName(folderPosition.Value.SourcePosition);
                var from = folderPosition.Value.TempPosition;
                var to = folderPosition.Value.SourcePosition;

                _log($"[Folder][Success] Save folder: \"{folderName}\" in path: {to}");
#if !FEATURE_DELETION_PROTECTION
                if (Directory.Exists(folderPosition.Value.SourceDirectory) == false)
                    Directory.CreateDirectory(folderPosition.Value.SourceDirectory);

                Directory.Move(from, to ?? throw new Exception("Unknown Path"));
#endif
            }
        }


        public bool ClearTempFiles()
        {
            var tempFolderPath = Path.GetTempPath();

            foreach (var file in Directory.GetFiles(tempFolderPath))
            {
                try
                {
#if !FEATURE_DELETION_PROTECTION
                    File.Delete(file);
#endif
                    _log($"[Folder][Success] Delete file: {file}");
                }
                catch (Exception e)
                {
                    _log($"[Folder][Error] Delete file {file}: {e.Message}");
                }
            }

            foreach (var directory in Directory.GetDirectories(tempFolderPath))
            {
                try
                {
#if !FEATURE_DELETION_PROTECTION
                    Directory.Delete(directory, true);
#endif
                    _log($"[Folder][Success] Delete directory: {directory}");
                }
                catch (Exception e)
                {
                    _log($"[Folder][Error] Delete directory {directory}: {e.Message}");
                }
            }

            return true;
        }
    }

    public struct RootObjectTransfer
    {
        public string SourceDirectory;
        public string SourcePosition;
        public string TempPosition;
    }
}