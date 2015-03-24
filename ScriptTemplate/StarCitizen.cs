using System.Threading.Tasks;
using System.Windows.Input;
using VOTCClient.Core;
using VOTCClient.Core.Helpers;
using VOTCClient.Core.Scripts;

namespace ScriptTemplate
{
    //Public functions / definations are forced. Everything else is up to you!
    public class StarCitizenTemplate
    {
        //Required! You need to fill out everything just like I did!
        //Copy paste this block to your scripts and edit it accordingly.

        public static bool Active;


        public static ScriptInfo SetUp()
        {
            new VoiceCommand().Create(new[] { "hi sally", "hello sally", "hey sally"}, "Hello " + Kernel.CustomName + "!");
            new VoiceCommand().Create("bye sally", "Bye " + Kernel.CustomName + "!");

            new VoiceCommand().Create("shields front", Key.NumPad8, "Transfering power", 2000);
            new VoiceCommand().Create("shields back", Key.NumPad2, "Transfering power", 2000);
            new VoiceCommand().Create("shields left", Key.NumPad4, "Transfering power", 2000);
            new VoiceCommand().Create("shields right", Key.NumPad6, "Transfering power", 2000);
            new VoiceCommand().Create("shields top", Key.NumPad9, "Transfering power", 2000);
            new VoiceCommand().Create("shields bottom", Key.NumPad3, "Transfering power", 2000);
            new VoiceCommand().Create("balance shields", Key.F5, "balancing power", 2000);

            new VoiceCommand().Create("eject", new[] { Key.LeftAlt, Key.L }, "Preparing evacuation sequence");
            new VoiceCommand().Create("respawn", Key.X, "Re-integrating your ship, please stand by");
            new VoiceCommand().Create("fire cm", Key.Y, "c m out");
            new VoiceCommand().Create("asta la vista baby", Key.D0, "c m out");

            new VoiceCommand().Create("toggle hud", Key.Home);
            new VoiceCommand().Create("toggle target lock", Key.LeftAlt, "locked on");
            new VoiceCommand().Create("toggle target focus", Key.L, "focus on target");
            new VoiceCommand().Create("toggle mouse look", new[] { Key.LeftCtrl, Key.C }, "control mode changed");

            new VoiceCommand().Create("free look on", Key.Tab, "control mode changed", KeyPressType.KeyDown);
            new VoiceCommand().Create("free look off", Key.Tab, "control mode changed", KeyPressType.KeyUp);

            new VoiceCommand().Create("power on group one", Key.D1, "transfering power", 2000);
            new VoiceCommand().Create("power on group two", Key.D2, "transfering power", 2000);
            new VoiceCommand().Create("power on group three", Key.D3, "transfering power", 2000);
            new VoiceCommand().Create("balance power", Key.D4, "balancing power", 2000);

            new VoiceCommand().Create("next enemy", Key.C, "Scanning");
            new VoiceCommand().Create("next ally", Key.H, "Scanning");
            new VoiceCommand().Create("next target", Key.C, "Scanning");
            new VoiceCommand().Create("next ally", Key.D0, "Scanning");
            new VoiceCommand().Create("next enemy", Key.C, "Scanning");

            new VoiceCommand().Create("match speed", Key.M, "velocity adaption finished");
            new VoiceCommand().Create("boost", Key.LeftShift, "sit tight", 5000);

            new VoiceCommand().Create("left roll", Key.A, "Sequence initiated",5200);
            new VoiceCommand().Create("right roll", Key.D, "Sequence initiated", 5200);

            Kernel.UI.DisplayCmd("Script loaded!", false);

            return new ScriptInfo
            {
                ScriptName = "Sally 0.1 - Basic Ship AI",
                Author = "BitFlash, LLC.",
                Description = "Very basic not yet state aware script that lets you control the majority of functions you use in your daily life as trader, miner or even pirate.",
                FriendlyGameName = "Star Citizen",
                ProcessName = "StarCitizen",
                RequireProcessInForeground = false,
                Commands = CommandStorage.AllCommands
            };
        }

        //Entry point. If we've heard something we let you know here!
        public static async Task<bool> IncommingVoicePacket(string voicePacket)
        {
            if (!Active && voicePacket == "hi sally")
                Active = true;
            if (Active && voicePacket == "bye sally")
                Active = false;

            if (Active)
            {
                return await VoiceCommand.ExecuteCommand(voicePacket);
            }
            Kernel.UI.DisplayCmd("Sally is not active. Say 'Hi Sally' to activate her");
            return false;
        }
    }
}