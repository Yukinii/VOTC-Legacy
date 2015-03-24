using System.IO;
using System.IO.Compression;

namespace Updater
{
    public static class Extensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (var file in archive.Entries)
            {
                var completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                if (file.Name == "Config.ini" || file.Name == "Updater.exe" || file.Name =="AutoDeploy.exe" || file.Name.Contains("vshost") || file.Name == "Log.txt")
                    continue;
                file.ExtractToFile(completeFileName, true);
            }
        }
    }
}
