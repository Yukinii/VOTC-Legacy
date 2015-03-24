using System;
using System.Collections.Concurrent;
using System.IO;

namespace BitFlashGenericWCF.Core
{
    public static class Extensions
    {
        public static string GetDirectoryName(this string file)
        {
            var directoryName = Path.GetDirectoryName(file);
            var path = directoryName?.Split('\\');
            return path?[path.Length - 1].Replace("\\", "");
        }
        public static void AddOrUpdate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value)
        {
            dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
        }
        public static string[] SplitByLine(this string s)
        {
            return s.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
