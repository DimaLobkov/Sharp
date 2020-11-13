using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherService
{
    class FileFeatures
    {
        public static void Archiving(string path)
        {
            using (FileStream sourceStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (FileStream outputStream = File.Create(path.Substring(0, path.LastIndexOf('.')) + ".gz"))
                {
                    using (GZipStream archiveStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(archiveStream);
                    }
                }
            }
            
            File.Delete(path);
        }

        public static void UnArchiving(string path)
        {
            using (FileStream sourceStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (FileStream outputStream = File.Create(path.Substring(0, path.LastIndexOf('.')) + ".txt"))
                {
                    using (GZipStream unarchiveStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        unarchiveStream.CopyTo(outputStream);
                    }
                }
            }

            File.Delete(path);
        }

        public static void Encrypt(string inputFile, string outputFile)
        {
            string password = @"myKey123";
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(password);
            byte[] arr = File.ReadAllBytes(inputFile);

            string cryptFile = outputFile;
            using (FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create))
            {
                RijndaelManaged RMCrypto = new RijndaelManaged();

                using (CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write))
                {
                    foreach (byte bt in arr)
                    {
                        cs.WriteByte(bt);
                    }
                }
            }
        }

        public static void Decrypt(string inputFile, string outputFile)
        {
            {
                string password = @"myKey123";

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);
                var bt = new List<byte>();

                using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                {
                    RijndaelManaged RMCrypto = new RijndaelManaged();

                    using (CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key),
                        CryptoStreamMode.Read))
                    {
                        int data;
                        while ((data = cs.ReadByte()) != -1)
                        {
                            bt.Add((byte)data);
                        }
                    }
                }
                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    foreach (byte b in bt)
                    {
                        fsOut.WriteByte(b);
                    }
                }
            }
        }


        public static void AddToArchive(string filePath)
        {
            string archivePath = "C:\\TargetD\\archive.zip";

            using (ZipArchive zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            }
        }
    }
}
