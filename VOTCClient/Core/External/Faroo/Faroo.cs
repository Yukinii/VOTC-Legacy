using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using VOTCClient.Core.Speech;

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
