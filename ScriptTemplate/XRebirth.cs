using System.Threading.Tasks;
using System.Windows.Input;
using VOTCClient.Core;
using VOTCClient.Core.Helpers;
using VOTCClient.Core.Scripts;

namespace ScriptTemplate
{
    public class XRebirth
    {
        public static ScriptInfo SetUp()
        {
            Kernel.UI.DisplayCmd("Please bind the following keys:", false);
            Kernel.UI.DisplayCmd("Full Reverse: Shift + 1", false);
            //Flight
            new VoiceCommand().Create("stop", Key.Back, "Thrust normalized");
            new VoiceCommand().Create("full speed", Key.Tab, "redirecting all power to main thrusters");
            new VoiceCommand().Create("reverse speed", new[] { Key.LeftShift, Key.D1 }, "redirecting all power to main thrusters");
            new VoiceCommand().Create("toggle control mode", Key.Space, "control mode changed");
            new VoiceCommand().Create("leave highway", Key.Z, "leaving highway");
            new VoiceCommand().Create("auto pilot", new[] { Key.LeftShift, Key.A }, "Missile launch initiated");

            //Weapons
            new VoiceCommand().Create("asta la vista baby", Key.L, "Missile launch initiated");
            new VoiceCommand().Create("next weapon", Key.N, "weapons switched");
            new VoiceCommand().Create("next missile", Key.M, "weapons switched");

            //Menus
            new VoiceCommand().Create("trade menu", new[] { Key.LeftShift, Key.T }, "Lets see what they got in store");
            new VoiceCommand().Create("poperty menu", new[] { Key.LeftShift, Key.P }, "I want a raise");
            new VoiceCommand().Create("Inventory", new[] { Key.LeftShift, Key.I }, "Please empty your pockets "+Kernel.CustomName);
            new VoiceCommand().Create("Abilities", Key.T, Kernel.CustomName+", Sir!");

            //Computer
            new VoiceCommand().Create("computer", Key.I, "Hey don't touch me");
            new VoiceCommand().Create("contact", Key.C, "Established secure link");
            new VoiceCommand().Create("sector map", Key.OemComma, "Yes sir");
            new VoiceCommand().Create("zone map", Key.OemPeriod, "Yes sir");
            new VoiceCommand().Create("dock", new[] { Key.LeftShift, Key.D }, "I'm not drunk, i swear.");
            new VoiceCommand().Create("closest enemy", new[] { Key.LeftShift, Key.E }, "Smoke him");
            new VoiceCommand().Create("target object", new[] { Key.LeftShift, Key.F }, "scanning");
            new VoiceCommand().Create("next target", Key.PageDown, "scanning");
            new VoiceCommand().Create("previous target", Key.PageDown, "scanning");
            new VoiceCommand().Create("radar", new[] { Key.LeftShift, Key.R }, "scanning");
            new VoiceCommand().Create("switch radar mode", new[] { Key.LeftShift, Key.M }, "scanning");

            //Save
            new VoiceCommand().Create("quick save", Key.F5, "saving");

            //View
            new VoiceCommand().Create("cockpit", Key.F1);
            new VoiceCommand().Create("external view", Key.F2);
            new VoiceCommand().Create("target camera", Key.F3);
            new VoiceCommand().Create("zoom in", Key.Add);
            new VoiceCommand().Create("zoom out", Key.Subtract);

            new VoiceCommand().Create("pause", Key.Pause, "See you later");
            new VoiceCommand().Create("close", Key.Escape);

            VoiceCommand.GenerateGrammar();

            var info = new ScriptInfo
            {
                ScriptName = "X Rebirth Basic 1.0",
                Author = "BitFlash, LLC.",
                Description = "This Script allows you to use the majority of actions you need on a day to day basis as merchant, miner or pirate",
                FriendlyGameName = "X4 Rebirth",
                ProcessName = "XRebirth",
                RequireProcessInForeground = false,
                Commands = CommandStorage.AllCommands
            };
            return info;
        }
        public static async Task<bool> IncommingVoicePacket(string voicePacket)
        {
            return await VoiceCommand.ExecuteCommand(voicePacket).ConfigureAwait(false);
        }
    }
}
