using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using VOTCClient.Core.IO;

namespace VOTCClient.Core.Extensions
{
    public static class Extensions
    {
        public static void AddOrUpdate<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key, TV value) => dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);

        /// <summary>
        /// Extension to convert human readable Key.Name into the brainfuck Forms.Keys value required for the keyboard hook.
        /// </summary>
        /// <param name="k">The Windows.Input.Key key</param>
        /// <returns>The Windows.Forms.Keys key</returns>
        public static Keys ToKey(this Key k) => (Keys)KeyInterop.VirtualKeyFromKey(k);

        public static Keys[] ToKey(this IEnumerable<Key> k) => k.Select(key => (Keys) KeyInterop.VirtualKeyFromKey(key)).ToArray();

        /// <summary>
        /// Splits by \n\r and \n
        /// </summary>
        /// <param name="s">string with line breaks</param>
        /// <returns>string array</returns>
        public static string[] SplitByLine(this string s) => s.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        public static string GetDirectoryName(this string file)
        {
            var directoryName = Path.GetDirectoryName(file);
            var path = directoryName?.Split('\\');
            return path?[path.Length - 1].Replace("\\", "");
        }

        public static bool ContainsMany(this List<string> list, params string[] parts) => parts.All(list.Contains);

        public static bool ContainsAny(this List<string> list, params string[] parts) => parts.Select(list.Contains).FirstOrDefault();

        /// <summary>
        /// Deletes a Directory and its Contents. Even ReadOnly files (run as admin)
        /// </summary>
        /// <param name="targetDir">Directory to delete</param>
        public static void DeleteDirectory(string targetDir)
        {
            try
            {
                var files = Directory.GetFiles(targetDir);
                var dirs = Directory.GetDirectories(targetDir);

                foreach (var f in files)
                {
                    File.SetAttributes(f, FileAttributes.Normal);
                    DeleteFile(f);
                }

                foreach (var dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(targetDir, false);
            }
            catch(Exception ex)
            {
                IoQueue.Add(ex);
            }
        }

        /// <summary>
        /// Tries to delete a file up to 250 times. It will sleep the calling thread for 100ms each failed attempt.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>True if deleted</returns>
        public static void DeleteFile(string path)
        {
            byte tries = 0;
            while (tries < 250)
            {
                tries++;
                try
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                    return;
                }
                catch (Exception ex)
                {
                    IoQueue.Add(ex);
                    Thread.Sleep(100);
                }
            }
            Kernel.UI.DisplayCmd("File " + path + " could not be deleted!");
        }
    }
}
