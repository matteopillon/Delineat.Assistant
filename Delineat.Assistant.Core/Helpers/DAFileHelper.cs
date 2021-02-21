using System.IO;

namespace Delineat.Assistant.Core.Helpers
{
    public class DAFileHelper
    {
        public static string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static string GetNotExistingFileName(string path)
        {
            int index = 1;
            string testPath = path;
            while (File.Exists(testPath))
            {
                testPath = $"{Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path))} ({index}){Path.GetExtension(path)}";
                index++;
            }
            return testPath;
        }
    }
}
