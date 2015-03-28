using System; //VOTC LEGACY
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BitFlashGenericWCF.Core;
using BitFlashGenericWCF.Database;
using BitFlashGenericWCF.Scripts;
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
namespace BitFlashGenericWCF
{
    public class Logic : ILogic
    {
        public string TestRemoteExecution(int value,string apiKey) => !API.Validate(apiKey) ? "Access Denied. API Key banned." : string.Format("You entered: {0}", value);

        public string DownloadScript(string scriptName, string apiKey)
        {
            Console.WriteLine("Download Script {0}", scriptName);
            return File.ReadAllText(Path.Combine(Kernel.ScriptLocation, scriptName));
        }

        public string UploadScript(string scriptName, string apiKey)
        {
            Console.WriteLine("UploadScript {0}", scriptName);
            var s = new Script();
            return s.ScriptSubmit(scriptName.SplitByLine(), scriptName.Contains("Socket") || scriptName.Contains(".Listen"));
        }

        public string StoreBadge(string scriptName, string apiKey)
        {
            Console.WriteLine("StoreBadge {0}", scriptName);
            return Kernel.AllScripts[scriptName].StoreBadge;
        }

        public string StoreHeader(string scriptName, string apiKey)
        {
            Console.WriteLine("StoreHeader {0}", scriptName);
            return Kernel.AllScripts[scriptName].HeaderImage;
        }

        public string GetScriptHash(string scriptName, string apiKey)
        {
            Console.WriteLine("GetScriptHash");
            return Kernel.AllScripts[scriptName].Updated.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
        }

        public List<string> LiveStats(string apiKey)
        {
            Console.WriteLine("LiveStats");
            return Kernel.CommandsReceived;
        }

        public List<string> GetMostDownloadedScripts(string apiKey)
        {
            Console.WriteLine("GetMostDownloadedScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.MostDownloaded).Select(script => script.Key).ToList();
        }

        public List<string> GetNewestScripts(string apiKey)
        {
            Console.WriteLine("GetNewestScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.New).Select(script => script.Key).ToList();
        }

        public List<string> GetHarmfulScripts(string apiKey)
        {
            Console.WriteLine("GetHarmfulScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.Harmful).Select(script => script.Key).ToList();
        }

        public List<string> GetMostPopularScripts(string apiKey)
        {
            Console.WriteLine("GetMostPopularScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.Popular).Select(script => script.Key).ToList();
        }

        public List<string> GetFeaturedScripts(string apiKey)
        {
            Console.WriteLine("GetFeaturedScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.Featured).Select(script => script.Key).ToList();
        }

        public List<string> GetUpdatedScripts(string apiKey)
        {
            Console.WriteLine("GetUpdatedScripts");
            return Kernel.AllScripts.Where(o => o.Value.Category == Category.Updated).Select(script => script.Key).ToList();
        }

        public void PostChatMessage(string json)
        {
            Kernel.ChatMessages.Insert(0,json);
            if (Kernel.ChatMessages.Count > 20)
                Kernel.ChatMessages.RemoveAt(20);
        }
        
        public string GetQuote()
        {
            return Quotes.GetRandom();
        }
    }
}
