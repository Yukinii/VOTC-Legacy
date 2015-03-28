using System; //VOTC LEGACY
using System.IO;
using System.Threading.Tasks;
using BitFlashGenericWCF.Core;
using BitFlashGenericWCF.Scripts;
using Newtonsoft.Json;
/*
    This file is part of VOTC.

    VOTC is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VOTC is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VOTC.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace BitFlashGenericWCF.Database
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
                File.WriteAllText(Path.Combine(Kernel.ScriptLocation, scriptFile.Key), JsonConvert.SerializeObject(scriptFile.Value));
            }
            Kernel.WriteLine("[AutoSave] Saved all Scripts!", ConsoleColor.Cyan);
        }
    }
}