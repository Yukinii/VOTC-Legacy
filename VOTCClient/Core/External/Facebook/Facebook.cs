using System; //VOTC LEGACY
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
