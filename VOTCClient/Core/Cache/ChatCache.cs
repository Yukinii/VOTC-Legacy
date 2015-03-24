using System;
using System.IO;
using System.Net;
using System.Reflection;
using VOTCClient.Core.IO;

namespace VOTCClient.Core.Cache
{
    public static class ChatCache
    {
        public static async void CacheImage(string imageUrl, string username)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png"))
                        await client.DownloadFileTaskAsync(imageUrl, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png");
                }
                Kernel.ChatCache.TryAdd(imageUrl, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\" + username + ".png");
            }
            catch (Exception ex)
            {
                IoQueue.Add(ex);
            }
        }

        public static string CacheLookup(string url)
        {
            string value;
            return Kernel.ChatCache.TryGetValue(url, out value) ? value : "";
        }
    }
}
