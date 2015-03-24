using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows;

namespace VOTCClient.Core.Speech
{
    public class TextToSpeech
    {
        private static int _volume;
        private static int _speed;
        public static bool UseTts { internal set; get; }

        public static int Volume
        {
            internal set
            {
                _volume = value;
                if (Speaker != null)
                    Speaker.Volume = _volume;
            }
            get { return _volume; }
        }
        public static int Speed
        {
            internal set
            {
                _speed = value;
                if (Speaker != null)
                    Speaker.Rate = _speed;
            }
            get { return _speed; }
        }

        internal static string VoiceName;
        internal static SpeechSynthesizer Speaker;
        internal static readonly Queue<string> SpeakQueue = new Queue<string>();

        internal static void PrepareTextToSpeech()
        {
            Speaker = new SpeechSynthesizer();
            Speaker.SetOutputToDefaultAudioDevice();
            Speaker.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            Speaker.SpeakCompleted += Speaker_SpeakCompleted;
            try
            {
                if (!string.IsNullOrEmpty(VoiceName))
                    Speaker.SelectVoice(VoiceName);
            }
            catch
            {
                Speaker.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            }
        }

        static void Speaker_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (SpeakQueue.Count > 0)
            Speak(SpeakQueue.Dequeue());
        }
        public static void Speak(string message)
        {
            try
            {
                if (Speaker == null)
                    PrepareTextToSpeech();

                if (!UseTts)
                    return;

                if (SpeakQueue.Count > 0)
                    SpeakQueue.Enqueue(message);
                else
                    Speaker?.SpeakAsync(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace);
            }
        }

        public static void PauseSpeaking() => Speaker.Pause();

        public static void ResumeSpeaking() => Speaker.Resume();

        public static void StopSpeaking()
        {
            Speaker.SpeakAsyncCancelAll();
            SpeakQueue.Clear();
        }
    }
}
