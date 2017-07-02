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
        public static readonly List<string> PluginBlackList = new List<string>(new[]
        {
            "AIMLbot.dll",
            "CommandLine.dll",
            "fmod.dll",
            "fmodstudio.dll",
            "IKVM.",
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
            "PrimMesher.dll",
            "RadSpeechLin.dll",
            "RadSpeechMac.dll",
            "RadSpeechWin.dll",
            "SmartThreadPool",
            "Tao.OpenGl.dll",
            "Tao.Platform.Windows.dll",
            "Tools.dll",
            "XMLRPC.dll",
            "zlib.net.dll",
        });

        /// <summary>List of file extensions that could potentially hold plugins</summary>
        public static readonly List<string> AllowedPluginExtensions = new List<string>(new[]
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
            this.Instance = instance;
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
                        Logger.Log($"ERROR unloading plugin: {plugin.Plugin.GetType().Name} because {ex}", Helpers.LogLevel.Warning, ex);
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
                var pluginsToUnload = Plugins.FindAll(loadedPlugin => loadedPlugin.Plugin == plugin.Plugin);
                foreach (var pluginToUnload in pluginsToUnload)
                {
                    var currentPluginDomain = pluginToUnload.Domain;
                    try
                    {
                        pluginToUnload.Plugin.StopPlugin(Instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR in unloading plugin: {pluginToUnload.Plugin.GetType().Name} because {ex}", Helpers.LogLevel.Warning, ex);
                    }
                    Plugins.Remove(pluginToUnload);

                    // Unload the domain housing the plugin we just unloaded if it was the last plugin in it.
                    var pluginWasInADomain = currentPluginDomain != null;
                    var domainIsNowEmpty = Plugins.Find(loadedPlugin => loadedPlugin.Domain == currentPluginDomain) == null;

                    if (pluginWasInADomain && domainIsNowEmpty)
                    {
                        try
                        {
                            AppDomain.Unload(currentPluginDomain);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"ERROR unloading application domain for : {plugin.FileName}\n{ex.Message}", Helpers.LogLevel.Warning);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Starts all loaded plugins
        /// </summary>
        public void StartPlugins()
        {
            lock (Plugins)
            {
                foreach (var plugin in Plugins)
                {
                    Logger.DebugLog($"Starting {plugin.Plugin.GetType().FullName}");
                    try
                    {
                        plugin.Start(Instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR in Starting Radegast Plugin: {plugin} because {ex}", Helpers.LogLevel.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a plugin for a precompiled assembly or source file
        /// </summary>
        /// <param name="path">File to load</param>
        /// <param name="startPlugins">Start plugins that are found in the assembly</param>
        public void LoadPluginFile(string path, bool startPlugins)
        {
            var extension = Path.GetExtension(path)?.ToLower();
            switch (extension)
            {
                case ".cs":
                    LoadCSharpScriptFile(path, startPlugins);
                    break;
                case ".dll":
                case ".exe":
                    LoadAssembly(path, startPlugins);
                    break;
            }
        }

        /// <summary>
        /// Scans and load plugins from Radegast application folder without starting them
        /// </summary>
        public void ScanAndLoadPlugins()
        {
            try
            {
                var pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (string.IsNullOrEmpty(pluginDirectory))
                {
                    return;
                }

                foreach (var pluginPath in Directory.GetFiles(pluginDirectory))
                {
                    if (IsBlacklisted(pluginPath))
                    {
                        continue;
                    }

                    try
                    {
                        LoadPluginFile(pluginPath, false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"ERROR unable to load plugin: {pluginPath} because {ex}", Helpers.LogLevel.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ERROR scanning and loading plugins: {ex}", Helpers.LogLevel.Warning);
            }
        }

        /// <summary>
        /// Determines if the specified file path is blacklisted and should not be loaded.
        /// </summary>
        /// <param name="pluginPath">File path to check.</param>
        /// <returns>True if the file path is blacklisted and should not be loaded.</returns>
        private static bool IsBlacklisted(string pluginPath)
        {
            var extension = Path.GetExtension(pluginPath)?.ToLower();
            if (string.IsNullOrEmpty(extension))
            {
                return true;
            }

            if (!AllowedPluginExtensions.Contains(extension))
            {
                return true;
            }

            var filename = Path.GetFileName(pluginPath)?.ToLower();
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
        public bool IsPluginLoaded(string pluginPath)
        {
            lock (Plugins)
            {
                return Plugins.Find(plugin => plugin.FileName == pluginPath) != null;
            }
        }

        /// <summary>
        /// Compiles and loads a plugin from a C# source file
        /// </summary>
        /// <param name="pluginPath">The source file to load</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadCSharpScriptFile(string pluginPath, bool startPlugins)
        {
            var source = File.ReadAllText(pluginPath);
            LoadCSharpScript(pluginPath, source, startPlugins);
        }

        /// <summary>
        /// Compiles and loads a plugin from a C# source file
        /// </summary>
        /// <param name="pluginPath">File name from which source was loaded</param>
        /// <param name="source">Source code</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadCSharpScript(string pluginPath, string source, bool startPlugins)
        {
            // *** Generate dynamic compiler
            var compilerOptions = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };
            var compiler = new CSharpCodeProvider(compilerOptions);
            var compilerParameters = new CompilerParameters();

            // *** Start by adding any referenced assemblies
            compilerParameters.ReferencedAssemblies.AddRange(new[] {
                "LibreMetaverse.StructuredData.dll",
                "LibreMetaverse.Types.dll",
                "LibreMetaverse.dll",
                "Radegast.exe",
                "System.dll",
                "System.Core.dll",
                "System.Xml.dll",
                "System.Drawing.dll",
                "System.Windows.Forms.dll",
            });

            // *** Load the resulting assembly into memory
            compilerParameters.GenerateInMemory = true;
            compilerParameters.GenerateExecutable = false;

            // *** Now compile the whole thing
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
                return;
            }

            Instance.TabConsole.DisplayNotificationInChat("Compilation successful.");
            LoadAssembly(pluginPath, compilerResults.CompiledAssembly, startPlugins);
        }

        /// <summary>
        /// Scans assembly for supported types
        /// </summary>
        /// <param name="assemblyPath">Path to the assembly</param>
        /// <param name="startPlugins">Start plugins found in the specified assembly after they're loaded</param>
        public void LoadAssembly(string assemblyPath, bool startPlugins)
        {
            LoadAssembly(assemblyPath, null, startPlugins);
        }

        /// <summary>
        /// Scans assembly for supported types
        /// </summary>
        /// <param name="assemblyPath">Path to the assembly to load or the path to where pluginAssembly was loaded from.</param>
        /// <param name="pluginAssembly">Assembly to load plugins from. May be null to specify the assembly needs to be loaded from assemblyPath.</param>
        /// <param name="startPlugins">Start plugins found in the specified assembly after they're loaded.</param>
        public void LoadAssembly(string assemblyPath, Assembly pluginAssembly, bool startPlugins)
        {
            if (IsPluginLoaded(assemblyPath))
            {
                Logger.Log($"Plugin already loaded, skipping: {assemblyPath}", Helpers.LogLevel.Info);
                if (startPlugins)
                {
                    Instance.TabConsole.DisplayNotificationInChat($"Plugin already loaded, skipping: {assemblyPath}");
                }
                return;
            }

            if (pluginAssembly == null)
            {
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
            }

            foreach (var type in pluginAssembly.GetTypes())
            {
                if (typeof(IRadegastPlugin).IsAssignableFrom(type))
                {
                    if (type.IsInterface)
                    {
                        continue;
                    }

                    LoadPluginFromType(assemblyPath, startPlugins, type);
                }
                else
                {
                    Instance.CommandsManager.LoadType(type);
                    Instance.ContextActionManager.LoadType(type);
                }
            }
        }

        private void LoadPluginFromType(string pluginPath, bool startPlugins, Type pluginType)
        {
            IRadegastPlugin pluginInstance = null;

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
                    throw new Exception($"Cannot load {pluginPath} because {pluginType} has no usable constructor.");
                }
            }

            var newPlugin = new PluginInfo(pluginPath, pluginInstance, null);
            lock (Plugins)
            {
                Plugins.Add(newPlugin);
            }

            if (startPlugins && pluginInstance != null)
            {
                newPlugin.Start(Instance);
            }
        }
    }
}
