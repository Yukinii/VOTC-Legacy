using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using VOTCClient.Core.Speech;
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
namespace VOTCClient.Core.External.Faroo
{
    public class Faroo
    {
        public static async Task Test()
        {
            try
            {
                TextToSpeech.Speak("Getting news, one moment.");
                var client = new WebClient {Headers = {["User-Agent"] = "VOTC " + Assembly.GetExecutingAssembly().GetName().Version}};
                var file = await client.DownloadStringTaskAsync("http://www.faroo.com/api?q=star%20citizen&start=1&length=3&l=en&src=news&f=json&key=VucKfXHS3Wth1Gy2NF9g9WU7Nko_");
                dynamic newsObject = await JsonConvert.DeserializeObjectAsync(file);
                foreach (var result in newsObject.results)
                {
                    TextToSpeech.Speak(result.domain.ToString().Replace("www.","") + " wrote " + result.title +". Here is a preview: " + result.kwic.ToString());
                }
                TextToSpeech.Speak("That's all I have for now "+Kernel.FacebookName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: "+ ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
