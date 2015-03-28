using System; //VOTC LEGACY
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CSharp;
using VOTCClient.Core.IO;
using VOTCClient.Core.Network;

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
namespace VOTCClient.Core.Scripts
{
    internal class ScriptSettings
    {
        public string ScriptLocation = Environment.CurrentDirectory + "\\Scripts\\";
        public readonly ConcurrentDictionary<int, Type> Types = new ConcurrentDictionary<int, Type>();

        private void AddScriptType(Type type)
        {
            if (!Types.TryAdd(Types.Count, type))
                throw new Exception("Could not add type.");
        }

        public readonly ConcurrentDictionary<int, string> Namespaces = new ConcurrentDictionary<int, string>();

        public void AddNamespace(string Namespace)
        {
            if (!Namespaces.TryAdd(Namespaces.Count, Namespace))
                throw new Exception("Could not add type.");
        }
        public void AddTypesInAssembly(Assembly asm, string Namespace)
        {
            foreach (var T in GetTypesInNamespace(asm, Namespace))
                AddScriptType(T);
        }
        private static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace) => assembly.GetTypes().Where(T => string.Equals(T.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
    }

    internal class ScriptEngine
    {
        private readonly ScriptSettings _settings;
        public readonly ConcurrentDictionary<string, MethodInfo> CompiledMethods;

        public ScriptEngine(ScriptSettings settings)
        {
            _settings = settings;
            CompiledMethods = new ConcurrentDictionary<string, MethodInfo>();

            Compile();
        }

        public void Compile()
        {
            Kernel.UI?.DisplayCmd("Recompiling scripts...", false);
            try
            {
                var files = Directory.GetFiles(_settings.ScriptLocation, "*.cs");
                var preparedFiles = files.ToDictionary(file => "Scripts\\" + Path.GetFileName(file), File.ReadAllText);

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var cSharpCodeProvider = new CSharpCodeProvider();
                var compilerParameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    IncludeDebugInformation = false,
                    TempFiles = new TempFileCollection("Cache\\Temp", false)
                };

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        compilerParameters.ReferencedAssemblies.Add(assembly.Location);
                    }
                    catch (Exception ex)
                    {
                        IoQueue.Add(ex);
                    }
                }
                foreach (var type in _settings.Types.Values)
                    compilerParameters.ReferencedAssemblies.Add(Assembly.GetAssembly(type).Location);

                foreach (var file in preparedFiles)
                {
                    var results = cSharpCodeProvider.CompileAssemblyFromFile(compilerParameters, file.Key);
                    if (results.Errors.Count != 0)
                    {
                        foreach (var error in results.Errors.Cast<CompilerError>().Where(err => !err.IsWarning))
                        {
                            Kernel.UI?.DisplayCmd("Buggy Script: " + Path.GetFileName(file.Key));
                            if (Kernel.DebugMode)
                                Kernel.UI?.DisplayCmd(error + error.FileName);
                        }
                    }
                    else
                    {
                        Kernel.UI?.DisplayCmd("Successfully compiled " + Path.GetFileName(file.Key), false);
                        foreach (var method in (from compiledType in results.CompiledAssembly.GetTypes()
                                                from method in compiledType.GetMethods()
                                                where method.IsStatic
                                                select method).Where(reflectedMethod => reflectedMethod.ReflectedType != null))
                        {
                            if (method.Name == "IncommingVoicePacket")
                                CompiledMethods.TryAdd(Path.GetFileName(file.Key) + "IncomingVoicePacket", method);
                            if (method.Name == "SetUp")
                                CompiledMethods.TryAdd(Path.GetFileName(file.Key) + "SetUp", method);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (IoQueue.L != null)
                    IoQueue.Add(e);
            }
        }

        public async Task<bool> Execute(string words)
        {
            try
            {
                MethodInfo info;
                if (!CompiledMethods.TryGetValue(Path.GetFileName(Kernel.ActiveProfile) + "IncomingVoicePacket", out info))
                    return false;
                return await ((Task<bool>)info.Invoke(null, new object[] { words }));
            }
            catch
            {
                MethodInfo info;
                if (!CompiledMethods.TryGetValue(Path.GetFileName(Kernel.ActiveProfile) + "IncomingVoicePacket", out info))
                    return false;
                return (bool)info.Invoke(null, new object[] { words });
            }
        }
        public ScriptInfo SetUp()
        {
            try
            {
                MethodInfo info;
                if (!CompiledMethods.TryGetValue(Path.GetFileName(Kernel.ActiveProfile) + "SetUp", out info))
                    return null;
                if (Kernel.Tracking)
                    Tracking.Add("Loaded Profile: " + Kernel.ActiveProfile);
                return (ScriptInfo)info.Invoke(null, null);
            }
            catch { return null; }
        }
    }
}