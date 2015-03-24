using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BitFlashGenericWCF.Core;
using BitFlashGenericWCF.Database;
using Newtonsoft.Json;

namespace BitFlashGenericWCF.Scripts
{
    public enum Category
    {
        None = 0,
        Featured = 1,
        New = 2,
        Harmful = 3,
        Popular = 4,
        Updated = 5,
        MostDownloaded
    }
    public class Script
    {
        public Category Category;
        public DateTime Added;
        public DateTime Updated;
        public bool Harmful;
        public string Name;
        public string Author;
        public string App;
        public string Foreground;
        public string Description;
        public List<string> Commands;
        public string Contents;
        public string HeaderImage;
        public string StoreBadge;
        public string Password;
        public ulong Downloads;
        public ulong RatingsCount;
        public float Rating;
        [JsonIgnore]
        int _scriptStartIndex;

        public string ScriptSubmit(string[] incommingData, bool harm)
        {
            Commands = new List<string>();
            Harmful = harm;
            Name = incommingData[0];
            Author = incommingData[1];
            App = incommingData[2];
            Foreground = incommingData[3];
            Description = incommingData[4];
            HeaderImage = incommingData[5];
            StoreBadge = incommingData[6];
            Password = incommingData[7];


            Category = harm ? Category.Harmful : Category.New;


            for (var I = 8; I < 1000; I++)
            {
                var s = incommingData[I];
                _scriptStartIndex = I;
                if (s.Contains("using System"))
                    break;
                Commands.Add(s);
            }
            var builder = new StringBuilder();
            for (var I = _scriptStartIndex; I < incommingData.Length; I++)
            {
                builder.AppendLine(incommingData[I]);
            }
            Contents = builder.ToString();
            if (!File.Exists(Kernel.ScriptLocation + Name + ".cs"))
            {
                Kernel.WriteLine("New Script uploaded: " + Name, ConsoleColor.Blue);
                Added = DateTime.UtcNow;
                Downloads = 0;
                Rating = 0;
                RatingsCount = 0;
                var json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Kernel.ScriptLocation + Name + ".cs", json);
                Kernel.AllScripts.TryAdd(Name + ".cs", this);
                return "Script successfully uploaded!";
            }
            Kernel.WriteLine("Script " + Name + " existed, waiting for password check...", ConsoleColor.Blue);
            dynamic obj = JsonConvert.DeserializeObject(File.ReadAllText(Kernel.ScriptLocation + Name + ".cs"));
            if (obj.Password.ToString() == Password)
            {
                Updated = DateTime.UtcNow;
                Category = Category.Updated;
                var json = JsonConvert.SerializeObject(this);
                File.WriteAllText(Kernel.ScriptLocation + Name + ".cs", json);
                Script s;
                Kernel.AllScripts.TryRemove(Name + ".cs", out s);
                Kernel.WriteLine("Script " + Name + " successfully updated!", ConsoleColor.Blue);
                Kernel.AllScripts.AddOrUpdate(Name + ".cs", this);
                return "Script successfully updated!";
            }
            ScriptDb.SaveScripts();
            return "Script name in use / Wrong password for update";
        }
    }
}
