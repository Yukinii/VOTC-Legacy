using System.Collections.Concurrent;
using System.Linq;
using System.Speech.Recognition;
using System.Threading.Tasks;
using System.Windows.Input;
using VOTCClient.Core.Extensions;
using VOTCClient.Core.Speech;
using Keyboard = InputManager.Keyboard;
using Mouse = InputManager.Mouse;

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
namespace VOTCClient.Core.Helpers
{
    public enum KeyPressType
    {
        KeyDown = 1,
        KeyUp = 2
    }

    public static class CommandStorage
    {
        public static readonly ConcurrentDictionary<string, VoiceCommand> AllCommands = new ConcurrentDictionary<string, VoiceCommand>();
    }

    /// <summary>
    /// Please add a Command like "new VoiceCommand().Create()" as re-using the same object does NOT work!
    /// Commands will not be garbage collected while your script is active.
    /// </summary>
    public class VoiceCommand
    {
        internal Mouse.MouseKeys[] MouseKeys;
        internal Key[] Keys;
        internal string Response;
        internal int KeyDownDuration;

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="command">What the user should say</param>
        /// <param name="pressKeys">What we should press</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        /// <param name="duration">How long we should press the keys (Milliseconds)</param>
        public void Create(string command, Key[] pressKeys, string textToSpeechResponse = "", int duration = 0)
        {
            if (string.IsNullOrEmpty(command))
                return;

            command = command.ToLowerInvariant();
            Response = textToSpeechResponse;
            Keys = pressKeys;
            KeyDownDuration = duration;

            CommandStorage.AllCommands.TryAdd(command, this);
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="commands">What the user should say</param>
        /// <param name="pressKeys">What we should press</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        /// <param name="duration">How long we should press the keys (Milliseconds)</param>
        public void Create(string[] commands, Key[] pressKeys, string textToSpeechResponse = "", int duration = 0)
        {
            if (commands == null)
                return;
            
            foreach (var command in commands)
            {
                Response = textToSpeechResponse;
                Keys = pressKeys;
                KeyDownDuration = duration;

                CommandStorage.AllCommands.TryAdd(command.ToLowerInvariant(), this);
            }
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="command">What the user should say</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        public void Create(string command, string textToSpeechResponse = "")
        {
            if (string.IsNullOrEmpty(command))
                return;

            command = command.ToLowerInvariant();
            Response = textToSpeechResponse;

            CommandStorage.AllCommands.TryAdd(command, this);
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="commands">What the user says</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        public void Create(string[] commands, string textToSpeechResponse = "")
        {
            if (commands == null)
                return;

            foreach (var command in commands)
            {
                Response = textToSpeechResponse;
                CommandStorage.AllCommands.TryAdd(command.ToLowerInvariant(), this);
            }
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="command">What the user should say</param>
        /// <param name="pressKeys">What we should press</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        /// <param name="duration">How long we should press the keys (Milliseconds)</param>
        public void Create(string command, Key pressKeys, string textToSpeechResponse = "", int duration = 0)
        {
            if (string.IsNullOrEmpty(command))
                return;

            command = command.ToLowerInvariant();
            Response = textToSpeechResponse;
            Keys = new[] {pressKeys};
            KeyDownDuration = duration;

            CommandStorage.AllCommands.TryAdd(command, this);
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="command">What the user should say</param>
        /// <param name="pressOrReleaseKey">What we should press</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        /// <param name="pressType">if true, SaveButton will be pressed if false, SaveButton will be released</param>
        public void Create(string command, Key pressOrReleaseKey, string textToSpeechResponse, KeyPressType pressType)
        {
            if (string.IsNullOrEmpty(command))
                return;

            command = command.ToLowerInvariant();
            Response = textToSpeechResponse;
            Keys = new[] {pressOrReleaseKey};

            if (pressType == KeyPressType.KeyDown)
                KeyDownDuration = int.MaxValue;
            else
                KeyDownDuration = -1;

            CommandStorage.AllCommands.TryAdd(command, this);
        }

        /// <summary>
        /// Creates a new Voice Command :)
        /// </summary>
        /// <param name="command">What the user should say</param>
        /// <param name="pressKeys">What we should press</param>
        /// <param name="textToSpeechResponse">What we should say</param>
        public void Create(string command, Mouse.MouseKeys pressKeys, string textToSpeechResponse = "")
        {
            if (string.IsNullOrEmpty(command))
                return;

            command = command.ToLowerInvariant();
            Response = textToSpeechResponse;
            MouseKeys = new[] {pressKeys};

            CommandStorage.AllCommands.TryAdd(command, this);
        }

        public static void GenerateGrammar()
        {
            var temp = new ConcurrentDictionary<string, ConcurrentList<string>>();

            foreach (var command in CommandStorage.AllCommands)
            {
                ConcurrentList<string> choices;

                if (command.Key.Contains(" "))
                {
                    var words = command.Key.Split(' ');

                    if (!temp.TryGetValue(words[0], out choices))
                        choices = new ConcurrentList<string>();

                    for (var I = 1; I < words.Length; I++)
                        choices.Add(words[I]);

                    temp.AddOrUpdate(words[0], choices);
                }
                else
                {
                    if (!temp.TryGetValue(command.Key, out choices))
                        choices = new ConcurrentList<string>();

                    temp.AddOrUpdate(command.Key, choices);
                }
            }

            foreach (var single in temp)
            {
                var grammarBuilder = new GrammarBuilder();
                if (single.Value.Count != 0)
                    grammarBuilder = new GrammarBuilder(single.Key);
                var commandChoices = new Choices();
                if (single.Value.Count == 0)
                    commandChoices = new Choices(single.Key);
                else
                {
                    foreach (var word in single.Value)
                    {
                        commandChoices.Add(word);
                    }
                }
                grammarBuilder.Append(commandChoices);
                InternalSpeechRecognizer.LoadGrammar(new Grammar(grammarBuilder));
            }
        }

        public static async Task<bool> ExecuteCommand(string speechPacket) => await Task.Run(async () =>
        {
            foreach (var cmd in CommandStorage.AllCommands.Where(cmd => speechPacket == cmd.Key))
            {
                if (cmd.Value.Keys.Length != 0)
                {
                    if (cmd.Value.Keys.Length > 1)
                        Keyboard.ShortcutKeys(cmd.Value.Keys.ToKey());
                    else switch (cmd.Value.KeyDownDuration)
                    {
                        case 0:
                            Keyboard.KeyPress(cmd.Value.Keys[0].ToKey());
                            break;
                        case -1:
                            Keyboard.KeyUp(cmd.Value.Keys[0].ToKey());
                            break;
                        case int.MaxValue:
                            Keyboard.KeyDown(cmd.Value.Keys[0].ToKey());
                            break;
                        default:
                            Keyboard.KeyDown(cmd.Value.Keys[0].ToKey());
                            await Task.Delay(cmd.Value.KeyDownDuration);
                            Keyboard.KeyUp(cmd.Value.Keys[0].ToKey());
                            break;
                    }
                }
                else if (cmd.Value.MouseKeys.Length != 0)
                {
                    Mouse.PressButton(cmd.Value.MouseKeys[0]);
                }
                if (!string.IsNullOrEmpty(cmd.Value.Response))
                    TextToSpeech.Speak(cmd.Value.Response);
                return true;
            }
            return false;
        });
    }
}