using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public static class StoreCache
    {
        public static async Task<string> CacheLookup(string name)
        {

            try
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
            catch (Exception e)
            {
                return "";
            }
        }

        public static void Clear()
        {
            Directory.Delete("Cache\\Store\\", true);
        }
    }
}
