using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ConfigurationProvider;

namespace Lab3
{
    class Logger
    {
        private readonly Options configOptions;
        private bool isEnabled = true;
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        string path;
        public Logger()                                                                                                               
        {
            var optionsManager = new OptionsManager(AppDomain.CurrentDomain.BaseDirectory);
            configOptions = optionsManager.GetOptions<Options>();
            path = configOptions.PathToDirectory.SourceDirectory;
            watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            watcher.Filter = "*.txt";
            watcher.Created += Watcher_Created;
        }
        public void Start()
        {
            if (configOptions != null)
            {
                watcher.EnableRaisingEvents = true;
            }

            while (isEnabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            isEnabled = false;
        }
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            var dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                dirInfo.Create();
            var pathFile = Path.Combine(path, e.Name);
            var fileName = e.Name;
            var dateT = DateTime.Now;
            var subPath = Path.Combine(dateT.ToString("yyyy", DateTimeFormatInfo.InvariantInfo), dateT.ToString("MM", DateTimeFormatInfo.InvariantInfo), dateT.ToString("dd", DateTimeFormatInfo.InvariantInfo));
            var newPath = Path.Combine(path, subPath, $"{Path.GetFileNameWithoutExtension(fileName)}_" + $"{dateT.ToString(@"yyyy_MM_dd_HH_mm_ss", DateTimeFormatInfo.InvariantInfo)}" + $"{Path.GetExtension(fileName)}");

            dirInfo.CreateSubdirectory(subPath);
            File.Move(pathFile, newPath);
            var compressedPath = Path.ChangeExtension(newPath, "gz");
            var newCompressedPath = Path.Combine(configOptions.PathToDirectory.TargetDirectory, Path.GetFileName(compressedPath));
            var decompressedPath = Path.ChangeExtension(newCompressedPath, "txt");
            FileOperations.Compress(newPath, compressedPath);
            File.Move(compressedPath, newCompressedPath);
            FileOperations.Decompress(newCompressedPath, decompressedPath);
            FileOperations.DecryptFile(decompressedPath);
            FileOperations.AddToArchive(decompressedPath, Path.Combine(configOptions.PathToDirectory.TargetDirectory,configOptions.Archive.Name));
            File.Delete(newCompressedPath);
            File.Delete(decompressedPath);
            File.Delete(compressedPath);
        }
    }
    }
}
