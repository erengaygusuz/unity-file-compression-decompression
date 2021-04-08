using System.IO.Compression;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public static class FileProcessManager
{
    public static class Compression
    {
        public static string FilesFolderPath { get; set; }
        public static string TargetSavePath { get; set; }
        public static string ActiveFileName { get; private set; }
        private static Progress<int> Progression { get; set; }
        public static float PercentageOfProgression { get; set; }
        public static bool IsAnyFileCompress { get; private set; }
        public static GenericEvent Started { get; set; }
        public static GenericEvent Continue { get; set; }
        public static GenericEvent Finished { get; set; }
        public static List<string> FileExtensionList { get; set; }

        public static async void Start(UnityAction callback = null)
        {
            Started?.Invoke();

            Progression = new Progress<int>(value =>
            {
                PercentageOfProgression = value;
                Continue?.Invoke();

                if (value == 100)
                {
                    Finished?.Invoke();

                    if (callback != null)
                    {
                        callback();
                    }
                }
            });

            await Task.Run(() =>
            {
                CompressMultipleFilesToZipFiles(FilesFolderPath, TargetSavePath, Progression);
            });
        }

        private static void CompressMultipleFilesToZipFiles(string filesFolderPath, string targetSavePath, IProgress<int> progress)
        {
            if (filesFolderPath == targetSavePath)
            {
                throw new SamePathException("Selected file path and target path can not be the same!");
            }

            if (filesFolderPath == "")
            {
                throw new EmptyPathNameException("Selected file path can not be empty");
            }

            if (targetSavePath == "")
            {
                throw new EmptyPathNameException("Target path can not be empty");
            }

            ClearTempFiles(targetSavePath);

            List<string> fileInfos = FilePaths(filesFolderPath, FileExtensionList);

            if (fileInfos.Count > 0)
            {
                for (int j = 0; j < fileInfos.Count; j++)
                {
                    string fileName = Path.GetFileName(fileInfos[j]);

                    Directory.CreateDirectory(targetSavePath + "\\" + fileName);

                    string fileSourcePath = filesFolderPath + "\\" + fileName;
                    string fileDestinationPath = targetSavePath + "\\" + fileName + "\\" + fileName;

                    File.Copy(fileSourcePath, fileDestinationPath);

                    if (File.Exists(targetSavePath + "\\" + fileName + ".zip"))
                    {
                        File.Delete(targetSavePath + "\\" + fileName + ".zip");
                    }

                    ActiveFileName = fileName;

                    ZipFile.CreateFromDirectory(targetSavePath + "\\" + fileName, targetSavePath + "\\" + fileName + ".zip");

                    Directory.Delete(targetSavePath + "\\" + fileName, true);

                    var percentageComplete = ((j + 1) * 100) / fileInfos.Count;
                    progress.Report(percentageComplete);
                }
            }
        }

        public static List<string> FilePaths(string path, List<string> extensions)
        {
            var fileInfos = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => extensions.Contains(new FileInfo(file).Extension, StringComparer.OrdinalIgnoreCase)).ToArray();

            return fileInfos.ToList();
        }
    }

    public static class Decompression
    {
        public static string FilesFolderPath { get; set; }
        public static string TargetSavePath { get; set; }
        public static string ActiveFileName { get; private set; }
        private static Progress<int> Progression { get; set; }
        public static int PercentageOfProgression { get; private set; }
        public static bool IsAnyFileDecompress { get; private set; }
        public static bool DeleteFilesAfterDecompress { get; set; }
        public static GenericEvent Started { get; set; }
        public static GenericEvent Continue { get; set; }
        public static GenericEvent Finished { get; set; }

        public static async void Start(UnityAction callback = null)
        {
            Started?.Invoke();

            Progression = new Progress<int>(value =>
            {
                PercentageOfProgression = value;
                Continue?.Invoke();

                if (value == 100)
                {
                    Finished?.Invoke();

                    if (callback != null)
                    {
                        callback();
                    }
                }
            });

            await Task.Run(() =>
            {
                DecompressMultipleZipFiles(FilesFolderPath, TargetSavePath, Progression);
            });
        }

        private static void DecompressMultipleZipFiles(string filesFolderPath, string targetSavePath, IProgress<int> progress)
        {
            if (filesFolderPath == targetSavePath)
            {
                throw new SamePathException("Selected file path and target path can not be the same!");
            }

            if (filesFolderPath == "")
            {
                throw new EmptyPathNameException("Selected file path can not be empty");
            }

            if (targetSavePath == "")
            {
                throw new EmptyPathNameException("Target path can not be empty");
            }

            ClearTempFiles(targetSavePath);

            List<string> fileInfos = FilePaths(filesFolderPath, new List<string> { ".zip" });

            if (fileInfos.Count > 0)
            {
                IsAnyFileDecompress = true;

                for (int i = 0; i < fileInfos.Count; i++)
                {
                    string fileName = Path.GetFileName(fileInfos[i]);
                    string path = filesFolderPath + "\\" + fileName;

                    ActiveFileName = fileName;

                    ZipFile.ExtractToDirectory(path, targetSavePath);

                    var percentageComplete = ((i + 1) * 100) / fileInfos.Count;
                    progress.Report(percentageComplete);

                    if (DeleteFilesAfterDecompress)
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        public static List<string> FilePaths(string path, List<string> extensions)
        {
            var fileInfos = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => extensions.Contains(new FileInfo(file).Extension, StringComparer.OrdinalIgnoreCase)).ToArray();

            return fileInfos.ToList();
        }
    }

    private static void ClearTempFiles(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }

        else
        {
            Directory.CreateDirectory(path);
        }
    }

    public class SamePathException : Exception
    {
        public SamePathException(string message) : base(message){}
    }

    public class EmptyPathNameException : Exception
    {
        public EmptyPathNameException(string message) : base(message){}
    }
}