using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VOTCServer.Core;
using VOTCServer.Scripts;

namespace VOTCServer.Database
{
    public static class ScriptDb
    {
        public static void LoadScripts()
        {
            if (File.Exists("DoanloadCount.txt"))
                File.Create("DownloadCount.txt");
            File.WriteAllText("DownloadCount.txt", "Downloads: "+Kernel.DownloadCount);
            Parallel.ForEach(Directory.GetFiles(Kernel.ScriptLocation, "*.cs", SearchOption.AllDirectories), file =>
            {
                var s = new Script();
                s = JsonConvert.DeserializeObject<Script>(File.ReadAllText(file));
                if (s.Added.Year == 1)
                    s.Added = DateTime.Now;
                if (DateTime.Now < s.Added.AddDays(3) && s.Category != Category.Harmful &&
                    s.Category != Category.Popular && s.Category != Category.Featured)
                    s.Category = Category.New;

                // ReSharper disable once AssignNullToNotNullAttribute
                Kernel.AllScripts.TryAdd(Path.GetFileName(file), s);
            });
        }

        public static void SaveScripts()
        {
            Kernel.WriteLine("[AutoSave] Saving Script...", ConsoleColor.Cyan);
            foreach (var scriptFile in Kernel.AllScripts)
            {
                File.WriteAllText(scriptFile.Key, JsonConvert.SerializeObject(scriptFile.Value));
            }
            Kernel.WriteLine("[AutoSave] Saved all Scripts!", ConsoleColor.Cyan);
        }
    }
}