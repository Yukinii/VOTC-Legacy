using System;
using VOTCClient.Core.Speech;

namespace VOTCClient.Core.External.Facebook
{
    internal static class Facebook
    {
        public static void PostToWall(string text)
        {
            try
            {
                dynamic result = Kernel.FacebookClient.Post("me/feed", new { message = text });
                TextToSpeech.Speak("Updated your status successfully.");
            }
            catch (Exception)
            {
                TextToSpeech.Speak("Failed to update your face book status.");
            }
            finally
            {
                Kernel.FacebookPostWindow.Close();
                Kernel.FacebookPostWindow = null;
            }
        }
    }
}
