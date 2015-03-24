using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VOTCClient.Core.Cache
{
    public static class StoreCache
    {
        public static async Task<string> CacheLookup(string name)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Cache\\Store\\"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Cache\\Store\\");

            foreach (var cacheFile in Directory.EnumerateFiles(Environment.CurrentDirectory + "\\Cache\\Store\\").Where(cachedFile => Path.GetFileName(cachedFile) == name + ".badge"))
            {
                return cacheFile;
            }
            using (var client = new WebClient())
            {
                var imageUrl = await Kernel.Channel.StoreBadgeAsync(name, "");
                File.WriteAllBytes("Cache\\Store\\" + name + ".badge", await client.DownloadDataTaskAsync(imageUrl));
            }
            return "Cache\\Store\\" + name + ".badge";
        }
    }
}
