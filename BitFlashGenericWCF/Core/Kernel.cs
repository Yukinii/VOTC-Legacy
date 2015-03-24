using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BitFlashGenericWCF.Scripts;
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
namespace BitFlashGenericWCF.Core
{
    public static class Kernel
    {
        public const string ScriptLocation = @"C:\Dropbox\VOTC\Scripts\";

        public static readonly ConcurrentDictionary<string, Script> AllScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> NewScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> FeaturedScripts = new ConcurrentDictionary<string, Script>();
        public static readonly ConcurrentDictionary<string, Script> PopularScripts = new ConcurrentDictionary<string, Script>();
        public static List<string> CommandsReceived = new List<string>();
        public static int DownloadCount = 0;
        public static void WriteLine(object text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
