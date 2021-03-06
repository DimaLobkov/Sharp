﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using ConfigManager;

namespace FileWatcherService
{
    class Logger
    {
        private readonly Options configOptions;
        private bool isEnabled = true;
        FileSystemWatcher watcher;
        object control = new object();
        bool enabled = true;
        string sourcePath;
        string targetPath;

        public Logger()
        {
            OptionsManager optionsManager = new OptionsManager(AppDomain.CurrentDomain.BaseDirectory);
            configOptions = optionsManager.GetOptions<Options>();
            sourcePath = configOptions.PathToDirectory.SourceDirectory;
            targetPath = configOptions.PathToDirectory.TargetDirectory;
            watcher = new FileSystemWatcher(sourcePath);
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
            enabled = false;
        }

        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            DirectoryInfo directorySource = new DirectoryInfo(sourcePath);
            if (!directorySource.Exists)
            {
                directorySource.Create();
            }

            DirectoryInfo directoryTarget = new DirectoryInfo(targetPath);
            if (!directoryTarget.Exists)
            {
                directoryTarget.Create();
            }

            string fileName = e.Name;
            string filePath = e.FullPath;
            DateTime dateTime = DateTime.Now;
            string fullFilePath = Path.Combine($"{dateTime.ToString("yyyy", DateTimeFormatInfo.InvariantInfo)}",
                $"{dateTime.ToString("MM", DateTimeFormatInfo.InvariantInfo)}", $"{dateTime.ToString("DD", DateTimeFormatInfo.InvariantInfo)}");
            string finalSourcePath = Path.Combine(sourcePath, fullFilePath, $"{Path.GetFileNameWithoutExtension(fileName)}" +
                $"{dateTime.ToString("_yyyy_MM_DD_HH_mm_ss", DateTimeFormatInfo.InvariantInfo)}" +
                $"{Path.GetExtension(fileName)}");
            string compressedPath = Path.ChangeExtension(finalSourcePath, "gz");
            string newCompressedPath = Path.Combine(targetPath, Path.GetFileName(compressedPath));
            string decompressedPath = Path.ChangeExtension(newCompressedPath, "txt");

            directorySource.CreateSubdirectory(fullFilePath);
            File.Move(filePath, finalSourcePath);
            FileFeatures.Encrypt(finalSourcePath, finalSourcePath);
            FileFeatures.Archiving(finalSourcePath);
            File.Move(compressedPath, newCompressedPath);
            FileFeatures.UnArchiving(newCompressedPath);
            FileFeatures.Decrypt(decompressedPath, decompressedPath);
            FileFeatures.AddToArchive(decompressedPath, configOptions.PathToDirectory.TargetDirectory + configOptions.Archive.Name);
            File.Delete(newCompressedPath);
            File.Delete(decompressedPath);
            File.Delete(compressedPath);
        }
    }
}