// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using OpenMetaverse;
using Microsoft.CSharp;

namespace Radegast
{
    /// <summary>
    /// Handles loading Radegast plugins
    /// </summary>
    public class PluginManager : IDisposable
    {
        /// <summary>List of files that should not be scanned for plugins</summary>
        private static readonly List<string> PluginBlackList = new List<string>(new string[]
        {
            "AIMLbot.dll",
            "CommandLine.dll",
            "fmod.dll",
            "fmodstudio.dll",
            "IKVM.",
            "LibreMetaverse.PrimMesher.dll",
            "LibreMetaverse.Rendering.Meshmerizer.dll",
            "LibreMetaverse.StructuredData.dll",
            "LibreMetaverse.dll",
            "LibreMetaverseTypes.dll",
            "log4net.dll",
            "Meebey.SmartIrc4net.dll",
            "Monobjc.Cocoa.dll",
            "Monobjc.dll",
            "OpenCyc.dll",
            "openjpeg-dotnet-x86_64.dll",
            "openjpeg-dotnet.dll",
            "OpenMetaverse.Rendering.Meshmerizer.dll",
            "OpenMetaverse.StructuredData.dll",
            "OpenMetaverse.dll",
            "OpenMetaverseTypes.dll",
            "OpenTK",
            "OpenTK.GLControl.dll",
            "PrimMesher.dll",
            "protobuf-net.dll",
            "RadSpeechLin.dll",
            "RadSpeechMac.dll",
            "RadSpeechWin.dll",
            "SmartThreadPool",
            "Tao.OpenGl.dll",
            "Tao.Platform.Windows.dll",
            "Tools.dll",
            "XMLRPC.dll",
            "XmlRpcCs.dll",
            "zlib.net.dll",
        });

        /// <summary>Collection of assemblies that are referenced when compiling scripts.</summary>
        private static readonly string[] ReferencedAssemblies = new string[]
        {
            "LibreMetaverse.StructuredData.dll",
            "LibreMetaverse.Types.dll",
            "LibreMetaverse.dll",
            "Radegast.exe",
            "System.dll",
            "System.Core.dll",
            "System.Xml.dll",
            "System.Drawing.dll",
            "System.Windows.Forms.dll",
        };

        /// <summary>List of file extensions that could potentially hold plugins</summary>
        private static readonly List<string> AllowedPluginExtensions = new List<string>(new string[]
        {
            ".cs",
            ".dll",
            ".exe"
        });

        private RadegastInstance Instance { get; }

        /// <summary>Collection of all of the loaded plugins</summary>
        public List<PluginInfo> Plugins { get; } = new List<PluginInfo>();

        /// <summary>
        /// Creates new PluginManager
        /// </summary>
        /// <param name="instance">Radegast instance PluginManager is associated with</param>
        public PluginManager(RadegastInstance instance)
        {
            Instance = instance;
        }

        /// <summary>
        /// Unloads all plugins
        /// </summary>
        public void Dispose()
        {
            lock (Plugins)
            {
                // Make a copy so we can have something to iterate over while removing items from the original list.
                var pluginsToRemove = Plugins.ToList();
                foreach (var plugin in pluginsToRemove)
                {
                    try
                    {
                        UnloadPlugin(plugin);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR unloading plugin: {plugin} because {ex}", Helpers.LogLevel.Warning, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Unloads a plugin
        /// </summary>
        /// <param name="plugin">Plugin to unload</param>
        public void UnloadPlugin(PluginInfo plugin)
        {
            lock (Plugins)
            {
                var pluginsToUnload = Plugins.FindAll(loadedPlugin => plugin.FileName == loadedPlugin.FileName);
                foreach (var pluginToUnload in pluginsToUnload)
                {
                    try
                    {
                        pluginToUnload.Stop(Instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR stopping plugin: {pluginToUnload} because {ex}", Helpers.LogLevel.Warning, ex);
                    }
                    Plugins.Remove(pluginToUnload);
                }
            }
        }

        /// <summary>
        /// Loads and starts all plugins in the specified directory.
        /// </summary>
        /// <exception cref="Exception">On failure</exception>
        public void LoadPluginsInDirectory(string pluginDirectory)
        {
            if (string.IsNullOrEmpty(pluginDirectory))
            {
                return;
            }

            foreach (var pluginPath in Directory.GetFiles(pluginDirectory))
            {
                LoadPlugin(pluginPath);
            }
        }

        /// <summary>
        /// Loads and starts the specified plugin.
        /// </summary>
        /// <exception cref="Exception">On failure</exception>
        public void LoadPlugin(string pluginPath)
        {
            var newPlugins = new List<PluginInfo>();

            try
            {
                var foundPlugins = LoadPluginFile(pluginPath);
                newPlugins.AddRange(foundPlugins);
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR unable to load plugin: {pluginPath} because {ex}", Helpers.LogLevel.Warning);
            }

            StartPlugins(newPlugins);
        }

        /// <summary>
        /// Starts the specified plugins.
        /// </summary>
        /// <param name="pluginsToStart">Plugins to start.</param>
        /// <exception cref="Exception">On failure</exception>
        private void StartPlugins(IEnumerable<PluginInfo> pluginsToStart)
        {
            lock (Plugins)
            {
                foreach (var newPlugin in pluginsToStart)
                {
                    Logger.DebugLog($"Starting {newPlugin}");
                    try
                    {
                        newPlugin.Start(Instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR in Starting Radegast Plugin: {newPlugin} because {ex}", Helpers.LogLevel.Warning);
                    }
                    Plugins.Add(newPlugin);
                }
            }
        }

        /// <summary>
        /// Loads plugins from the specified assembly or script.
        /// </summary>
        /// <param name="path">Path to an assembly or script file containing plugins.</param>
        /// <returns>List of plugins that were successfully loaded.</returns>
        /// <exception cref="Exception">On failure</exception>
        private List<PluginInfo> LoadPluginFile(string path)
        {
            if (IsBlacklisted(path))
            {
                return new List<PluginInfo>();
            }

            if (IsPluginLoaded(path))
            {
                Instance.TabConsole.DisplayNotificationInChat($"Plugin already loaded, skipping: {path}");
                return new List<PluginInfo>();
            }

            var extension = Path.GetExtension(path)?.ToLower();
            switch (extension)
            {
                case ".cs":
                    return LoadScript(path);
                case ".dll":
                case ".exe":
                    return LoadAssembly(path);
            }

            return new List<PluginInfo>();
        }

        /// <summary>
        /// Determines if the specified file path is blacklisted and should not be loaded.
        /// </summary>
        /// <param name="path">File path to check.</param>
        /// <returns>True if the file path is blacklisted and should not be loaded.</returns>
        private static bool IsBlacklisted(string path)
        {
            var extension = Path.GetExtension(path)?.ToLower();
            if (string.IsNullOrEmpty(extension))
            {
                return true;
            }

            if (!AllowedPluginExtensions.Contains(extension))
            {
                return true;
            }

            var filename = Path.GetFileName(path)?.ToLower();
            if (string.IsNullOrEmpty(filename))
            {
                return true;
            }

            return PluginBlackList.Any(blackListItem => filename.StartsWith(blackListItem.ToLower()));
        }

        /// <summary>
        /// Determines if the plugin at the specified path has already been loaded.
        /// </summary>
        /// <param name="pluginPath">Path to the plugin to check.</param>
        /// <returns>True if the plugin has already been loaded.</returns>
        private bool IsPluginLoaded(string pluginPath)
        {
            lock (Plugins)
            {
                return Plugins.Find(plugin => plugin.FileName == pluginPath) != null;
            }
        }

        /// <summary>
        /// Compiles and loads plugins from a script
        /// </summary>
        /// <param name="scriptPath">Path to the script to compile and load plugins from.</param>
        /// <returns>List of plugins that were successfully loaded.</returns>
        /// <exception cref="Exception">On failure</exception>
        private List<PluginInfo> LoadScript(string scriptPath)
        {
            // *** Generate dynamic compiler
            var compilerOptions = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };
            var compiler = new CSharpCodeProvider(compilerOptions);
            var compilerParameters = new CompilerParameters();

            // *** Start by adding any referenced assemblies
            compilerParameters.ReferencedAssemblies.AddRange(ReferencedAssemblies);

            // *** Load the resulting assembly into memory
            compilerParameters.GenerateInMemory = true;
            compilerParameters.GenerateExecutable = false;

            // *** Now compile the whole thing
            var source = File.ReadAllText(scriptPath);
            var compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, source);

            // *** Check for compilation erros
            if (compilerResults.Errors.HasErrors)
            {
                var errorMessage = new StringBuilder();

                errorMessage.AppendLine($"Compilation failed: {compilerResults.Errors.Count} errors:");
                for (var i = 0; i < compilerResults.Errors.Count; i++)
                {
                    errorMessage.AppendLine($"Line: {compilerResults.Errors[i].Line} - {compilerResults.Errors[i].ErrorText}");
                }

                Instance.TabConsole.DisplayNotificationInChat(errorMessage.ToString(), ChatBufferTextStyle.Alert);
                return new List<PluginInfo>();
            }

            Instance.TabConsole.DisplayNotificationInChat("Compilation successful.");
            return LoadPluginsFromAssembly(scriptPath, compilerResults.CompiledAssembly);
        }

        /// <summary>
        /// Loads all plugins from the specified assembly path.
        /// </summary>
        /// <param name="assemblyPath">Path to the assembly to load plugins from.</param>
        /// <returns>List of plugins that were successfully loaded.</returns>
        /// <exception cref="Exception">On failure</exception>
        private List<PluginInfo> LoadAssembly(string assemblyPath)
        {
            Assembly pluginAssembly = null;
            // We may try to load plugins from the currently running process. The assembly is already loaded.
            var currentProcessPath = Assembly.GetEntryAssembly().Location;
            if (Path.GetFileName(currentProcessPath) == Path.GetFileName(assemblyPath))
            {
                pluginAssembly = Assembly.GetEntryAssembly();
            }
            else
            {
                pluginAssembly = Assembly.LoadFile(assemblyPath);
            }

            return LoadPluginsFromAssembly(assemblyPath, pluginAssembly);
        }

        /// <summary>
        /// Loads all plugins from the specified assembly.
        /// </summary>
        /// <param name="assemblyPath">Path the assembly was loaded from.</param>
        /// <param name="pluginAssembly">Assembly to load plugins from.</param>
        /// <returns>List of plugins that were successfully loaded.</returns>
        /// <exception cref="Exception">On failure.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="pluginAssembly"/> is <see langword="null"/></exception>
        private List<PluginInfo> LoadPluginsFromAssembly(string assemblyPath, Assembly pluginAssembly)
        {
            if (pluginAssembly == null)
            {
                throw new ArgumentNullException(nameof(pluginAssembly));
            }

            var loadedPlugins = new List<PluginInfo>();

            foreach (var type in pluginAssembly.GetTypes())
            {
                if (typeof(IRadegastPlugin).IsAssignableFrom(type))
                {
                    if (type.IsInterface)
                    {
                        continue;
                    }

                    try
                    {
                        var newPlugin = LoadPluginFromType(assemblyPath, type);
                        loadedPlugins.Add(newPlugin);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Cannot load {type.Name} from {pluginAssembly.FullName} because: {ex}", Helpers.LogLevel.Warning);
                    }
                }
                else
                {
                    Instance.CommandsManager.LoadType(type);
                    Instance.ContextActionManager.LoadType(type);
                }
            }

            return loadedPlugins;
        }

        /// <summary>
        /// Instantiates a new plugin from the specified type.
        /// </summary>
        /// <param name="pluginPath">Path to the plugin or script.</param>
        /// <param name="pluginType">Type to instantiate a new plugin from.</param>
        /// <returns>Plugin that was loaded from the specified type.</returns>
        /// <exception cref="Exception">On failure.</exception>
        private PluginInfo LoadPluginFromType(string pluginPath, Type pluginType)
        {
            IRadegastPlugin pluginInstance;

            // Does the assembly have a constructor that has a RadegastInstance parameter?
            var constructorInfo = pluginType.GetConstructor(new Type[] { typeof(RadegastInstance) });
            if (constructorInfo != null)
            {
                pluginInstance = (IRadegastPlugin) constructorInfo.Invoke(new object[] {Instance});
            }
            else
            {
                // Does the assembly have a constructor?
                constructorInfo = pluginType.GetConstructor(Type.EmptyTypes);
                if (constructorInfo != null)
                {
                    pluginInstance = (IRadegastPlugin) constructorInfo.Invoke(null);
                }
                else
                {
                    throw new Exception($"No suitable constructor found in {pluginType.Name}.");
                }
            }

            return new PluginInfo(pluginPath, pluginInstance);
        }
    }
}
