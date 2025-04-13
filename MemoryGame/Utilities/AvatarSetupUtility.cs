using System;
using System.IO;
using System.Reflection;

namespace MemoryGame.Utilities
{
    public static class AvatarSetupUtility
    {
        public static void EnsureAvatarImages()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string avatarsDir = Path.Combine(baseDir, "Avatars");

            if (!Directory.Exists(avatarsDir))
            {
                Directory.CreateDirectory(avatarsDir);
            }

            if (Directory.GetFiles(avatarsDir, "*.jpg").Length == 0 &&
                Directory.GetFiles(avatarsDir, "*.jpeg").Length == 0 &&
                Directory.GetFiles(avatarsDir, "*.png").Length == 0 &&
                Directory.GetFiles(avatarsDir, "*.gif").Length == 0)
            {
                CopyImagesFromProjectFolder();
            }
        } 

        public static void CopyImagesFromProjectFolder()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string avatarsDir = Path.Combine(baseDir, "Avatars");

            if (!Directory.Exists(avatarsDir))
            {
                Console.WriteLine($"Directory not found: {avatarsDir}");
                return;
            }

            string sourceDir = Path.Combine(baseDir, "DefaultAvatars");

            if (Directory.Exists(sourceDir))
            {
                foreach (string sourceFile in Directory.GetFiles(sourceDir, "*.*"))
                {
                    string extension = Path.GetExtension(sourceFile).ToLower();
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                    {
                        string fileName = Path.GetFileName(sourceFile);
                        string destFile = Path.Combine(avatarsDir, fileName);

                        if (!File.Exists(destFile))
                        {
                            File.Copy(sourceFile, destFile);
                        }
                    }
                }
            }
        }
    }
}