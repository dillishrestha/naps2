﻿using System.IO.Compression;

namespace NAPS2.Dependencies;

public abstract class DownloadFormat
{
    public static DownloadFormat Gzip = new GzipDownloadFormat();

    public static DownloadFormat Zip = new ZipDownloadFormat();

    public abstract string Prepare(string tempFilePath);

    private class GzipDownloadFormat : DownloadFormat
    {
        public override string Prepare(string tempFilePath)
        {
            if (!tempFilePath.EndsWith(".gz", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException();
            }
            var pathWithoutGz = tempFilePath.Substring(0, tempFilePath.Length - 3);
            Extract(tempFilePath, pathWithoutGz);
            return pathWithoutGz;
        }

        private static void Extract(string sourcePath, string destPath)
        {
            using FileStream inFile = new FileInfo(sourcePath).OpenRead();
            using FileStream outFile = File.Create(destPath);
            using GZipStream decompress = new GZipStream(inFile, CompressionMode.Decompress);
            decompress.CopyTo(outFile);
        }
    }

    private class ZipDownloadFormat : DownloadFormat
    {
        public override string Prepare(string tempFilePath)
        {
            if (!tempFilePath.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException();
            }

            var tempDir = Path.GetDirectoryName(tempFilePath) ?? throw new ArgumentNullException();
            ZipFile.ExtractToDirectory(tempFilePath, tempDir);
            File.Delete(tempFilePath);
            return tempDir;
        }
    }
}