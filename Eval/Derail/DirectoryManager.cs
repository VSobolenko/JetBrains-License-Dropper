using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eval.Derail
{
    public class DirectoryManager
    {
        private readonly Action<string> _log;
        private readonly Dictionary<string, FileTransfer> _keepFiles = new Dictionary<string, FileTransfer>();

        public DirectoryManager(Action<string> log) => _log = log;

        public bool ClearFolders(IEnumerable<FolderData> positions)
        {
            var allPathsDelete = true;
            foreach (var position in positions)
                allPathsDelete = DeleteFolder(position.GetPath());

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

        private bool DeleteDirectory(string path2Folder)
        {
            if (Directory.Exists(path2Folder) == false)
                return false;

            _log($"[Folder][Success] Delete key path: {path2Folder}");
#if !FEATURE_DELETION_PROTECTION
            Directory.Delete(path2Folder, true);
#endif
            return true;
        }

        public void KeepFile(string folderPath, string fileName)
        {
            var file = new FileTransfer
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
            _keepFiles[fileName] = file;

#if !FEATURE_DELETION_PROTECTION
            File.Move(file.SourcePosition, file.TempPosition);
#endif
        }

        public IEnumerable<FileTransfer> ReturnFilesToSourcePositions()
        {
            foreach (var file in _keepFiles)
            {
                var folder = file.Value.SourceDirectory;
                
#if !FEATURE_DELETION_PROTECTION
                if (Directory.Exists(folder) == false)
                    Directory.CreateDirectory(folder);
#endif
                
                _log($"[Folder][Success] Save file: \"{file.Key}\" in path: {file.Value.SourcePosition}");
#if !FEATURE_DELETION_PROTECTION
                File.Move(file.Value.TempPosition, file.Value.SourcePosition);
#endif
            }

            var transfers = _keepFiles.Select(x => x.Value);
            _keepFiles.Clear();
            return transfers;
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

    public struct FileTransfer
    {
        public string SourceDirectory;
        public string SourcePosition;
        public string TempPosition;
    }
}