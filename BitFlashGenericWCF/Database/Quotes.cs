using System;
using System.Collections.Generic;
using System.IO;

namespace BitFlashGenericWCF.Database
{
    public static class Quotes
    {
        public static Random Rand = new Random(Environment.TickCount);
        public static List<string> AllQuotes = new List<string>(1000);
        public static void Load()
        {
            var quotes = File.ReadAllLines("Quotes.txt");
            foreach (var quote in quotes)
            {
                AllQuotes.Add(quote);
            }
        }

        public static string GetRandom()
        {
            if (AllQuotes.Count == 0)
                Load();
            var index = Rand.Next(0, AllQuotes.Count);
            Rand = new Random(Environment.TickCount);
            return AllQuotes[index];
        }
    }
}
