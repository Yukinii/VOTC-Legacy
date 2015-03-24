using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Windows;
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
namespace VOTCClient.Core.Speech
{
    public static class InternalSpeechRecognizer
    {
        internal static bool IsNaturalSpeaking;
        internal static bool DisplayUnknownCommands = true;
        internal static int MinimumConfidence = 80;
        internal static SpeechRecognitionEngine Engine;
        internal static readonly DictationGrammar Dictation = new DictationGrammar();
        internal static void PrepareSpeech()
        {
            if(Engine!=null)
                return;
            try
            {
                Engine = new SpeechRecognitionEngine
                {
                    InitialSilenceTimeout = TimeSpan.FromSeconds(0.1),
                    BabbleTimeout = TimeSpan.FromSeconds(0.1),
                    EndSilenceTimeout = TimeSpan.FromSeconds(0.1),
                    EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(0.1)
                };
                Engine.LoadGrammar(new Grammar(new GrammarBuilder(new Choices("delete track","pause music","resume music","clear","I accept lord gaben into my wallet","play music", "next track", "stop music", "lock", "unlock","activate push to talk", "deactivate push to talk","turn up volume","turn down volume"))));
                Engine.SetInputToDefaultAudioDevice();
                if (IsNaturalSpeaking)
                    EnableNaturalSpeaking();
                Engine.SpeechRecognized += Recognized;
                Engine.AudioLevelUpdated += Engine_AudioLevelUpdated;

                if (!Kernel.UsePtt)
                    Engine.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                MessageBox.Show("Looks like you don't have a microphone connected to your PC!");
            }
        }
        static void Engine_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e) => Kernel.UI.UpdateVolume((e.AudioLevel));

        public static void LoadGrammar(Grammar builder) => Engine?.LoadGrammar(builder);

        public static void UnLoadGrammar(Grammar builder) => Engine?.UnloadGrammar(builder);

        public static void LoadGrammar(IEnumerable<Grammar> builder)
        {
            if (Engine == null)
                return;
            foreach (var grammar in builder)
            {
                Engine.LoadGrammar(grammar);
            }
        }
        public static void UnLoadGrammar(IEnumerable<Grammar> builder)
        {
            if (Engine == null)
                return;
            foreach (var grammar in builder)
            {
                Engine.UnloadGrammar(grammar);
            }
        }

        internal static void EnableNaturalSpeaking()
        {
            if (Engine == null)
                return;
            if (!Engine.Grammars.Contains(Dictation))
                Engine.LoadGrammar(Dictation);
        }
        internal static void DisableNaturalSpeaking()
        {
            if (Engine == null)
                return;
            if (Engine.Grammars.Contains(Dictation))
                Engine.UnloadAllGrammars();
        }

        static async void Recognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Kernel.Locked && e.Result.Text != "unlock")
                return;

            if (Kernel.DeveloperMode)
                Kernel.InitializeScriptEngine();
            if ((e.Result.Confidence*100) < MinimumConfidence)
                Kernel.UI.DisplayCmd("Not confident enough: '" + e.Result.Text + "' @ " + (int)(e.Result.Confidence * 100) + "%",false);
            else
                Kernel.UI.DisplayCmd(e.Result.Text, await SpeechPacketHandler.Process(e.Result.Text));
        }
    }
}
