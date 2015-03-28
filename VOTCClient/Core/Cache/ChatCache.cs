using System; //VOTC LEGACY
using System.IO;
using System.Net;
using System.Reflection;
using VOTCClient.Core.IO;
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

        public static string CacheLookup(string url,string name)
        {
            CacheImage(url, name);
            string value;
            return Kernel.ChatCache.TryGetValue(url, out value) ? value : "";
        }

        public static void Clear()
        {
            Kernel.ChatCache.Clear();
            Directory.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Cache\ChatCache\", true);
        }
    }
}
