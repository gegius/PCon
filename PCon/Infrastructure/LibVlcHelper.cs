using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace PCon.Infrastructure
{
    public class LibVlcHelper
    {
        public DirectoryInfo GetVlcPlayerPath()
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (!Directory.Exists(Path.Combine(currentDirectory, "libvlc")))
                ZipFile.ExtractToDirectory(Path.Combine(currentDirectory, "libvlc.zip"), currentDirectory);
            return new DirectoryInfo(Path.Combine(currentDirectory, "libvlc",
                Environment.Is64BitOperatingSystem ? "win-x64" : "win-x86"));
        }
    }
}