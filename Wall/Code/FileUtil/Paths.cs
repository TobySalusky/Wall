using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Wall {
    public static class Paths {

        public static string solutionPath, assetPath, texturePath, musicPath;

        static Paths() {
            var path = Path.GetFullPath("hi");
            var separator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\\" : "/";
            
            solutionPath = path.Substring(0, path.IndexOf($"bin{separator}Debug", StringComparison.Ordinal));
            assetPath = solutionPath + $"Assets{separator}";
            texturePath = assetPath + $"Textures{separator}";
            musicPath = assetPath + $"Music{separator}";
        }

    }
}