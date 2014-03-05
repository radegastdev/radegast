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
using System.Windows.Forms;
using OpenMetaverse;
using Microsoft.CSharp;

namespace Radegast
{
    /// <summary>
    /// Information about loaded plugin
    /// </summary>
    public class PluginInfo
    {
        /// <summary>File name from which the plugin was loaded, cannot load plugin twice from the same file</summary>
        public string FileName { get; set; }
        /// <summary>Plugin class</summary>
        public IRadegastPlugin Plugin { get; set; }
        /// <summary>Is plugin started</summary>
        public bool Started { get; set; }
        /// <summary>Plugin class</summary>
        public PluginAttribute Attribures
        {
            get
            {
                if (Plugin == null) return null;
                return PluginManager.GetAttributes(Plugin);
            }
        }

        public AppDomain Domain;
    }

    /// <summary>
    /// Handles loading Radegast plugins
    /// </summary>
    public class PluginManager : IDisposable
    {
        /// <summary>List of files that should not be scanned for plugins</summary>
        public static readonly List<string> PluginBlackList = new List<string>(new string[]
            {
                "AIMLbot.dll",
                "CommandLine.dll",
                "Meebey.SmartIrc4net.dll",
                "Monobjc.Cocoa.dll",
                "Monobjc.dll",
                "OpenMetaverse.Rendering.Meshmerizer.dll",
                "OpenMetaverse.StructuredData.dll",
                "OpenMetaverse.dll",
                "OpenMetaverseTypes.dll",
                "PrimMesher.dll",
                "RadSpeechLin.dll",
                "RadSpeechMac.dll",
                "RadSpeechWin.dll",
                "Tao.OpenGl.dll",
                "Tao.Platform.Windows.dll",
                "Tools.dll",
                "XMLRPC.dll",
                "fmodex-dotnet.dll",
                "fmodex.dll",
                "log4net.dll",
                "openjpeg-dotnet-x86_64.dll",
                "openjpeg-dotnet.dll",
                "OpenCyc.dll",
                "IKVM.",
                "OpenTK",
                "zlib.net.dll",
                "SmartThreadPool",
            });

        /// <summary>List of file extensions that could potentially hold plugins</summary>
        public static readonly List<string> AllowedPluginExtensions = new List<string>(new string[]
            {
                ".cs",
                ".dll",
                ".exe"
            });

        List<PluginInfo> PluginsLoaded = new List<PluginInfo>();
        RadegastInstance instance;

        /// <summary>
        /// Gets the list of currently loaded plugins
        /// </summary>
        public List<PluginInfo> Plugins
        {
            get
            {
                return PluginsLoaded;
            }
        }

        /// <summary>
        /// Creates new PluginManager
        /// </summary>
        /// <param name="instance">Radegast instance PluginManager is associated with</param>
        public PluginManager(RadegastInstance instance)
        {
            this.instance = instance;
        }

        /// <summary>
        /// Unloads all plugins
        /// </summary>
        public void Dispose()
        {
            lock (PluginsLoaded)
            {
                List<PluginInfo> unload = new List<PluginInfo>(PluginsLoaded);
                unload.ForEach(plug =>
                {
                    UnloadPlugin(plug);
                });
            }
        }

        /// <summary>
        /// Unloads a plugin
        /// </summary>
        /// <param name="plug">Plugin to unload</param>
        public void UnloadPlugin(PluginInfo plug)
        {
            lock (PluginsLoaded)
            {
                var pluginInfos = PluginsLoaded.FindAll(info => { return info.Plugin == plug.Plugin; });

                foreach (var info in pluginInfos)
                {
                    AppDomain domain = info.Domain;
                    try { info.Plugin.StopPlugin(instance); }
                    catch (Exception ex) { Logger.Log("ERROR in unloading plugin: " + info.Plugin.GetType().Name + " because " + ex, Helpers.LogLevel.Debug, ex); }
                    PluginsLoaded.Remove(info);

                    if (domain != null && PluginsLoaded.Find(dinfo => { return dinfo.Domain == domain; }) == null)
                    {
                        try { AppDomain.Unload(domain); }
                        catch (Exception ex) { Logger.Log("ERROR unloading application domain for : " + plug.FileName + "\n" + ex.Message, Helpers.LogLevel.Debug); }
                    }
                }
            }
        }

        /// <summary>
        /// Gets extended atributes for plugin
        /// </summary>
        /// <param name="plug">Plugin to lookup extra attributes</param>
        /// <returns>Extended atributes for plugin</returns>
        public static PluginAttribute GetAttributes(IRadegastPlugin plug)
        {
            PluginAttribute a = null;

            foreach (Attribute attr in Attribute.GetCustomAttributes(plug.GetType()))
            {
                if (attr is PluginAttribute)
                    a = (PluginAttribute)attr;
            }

            if (a == null)
            {
                a = new PluginAttribute();
                a.Name = plug.GetType().FullName;
            }

            return a;
        }

        /// <summary>
        /// Starts all loaded plugins
        /// </summary>
        public void StartPlugins()
        {
            lock (PluginsLoaded)
            {
                foreach (PluginInfo plug in PluginsLoaded)
                {
                    Logger.DebugLog("Starting " + plug.Plugin.GetType().FullName);
                    try
                    {
                        plug.Plugin.StartPlugin(instance);
                        plug.Started = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Starting Radegast Plugin: " + plug + " because " + ex, Helpers.LogLevel.Debug);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a plugin for a precompiled assembly or source file
        /// </summary>
        /// <param name="loadFileName">File to load</param>
        /// <param name="stratPlugins">Start plugins that are found in the assembly</param>
        public void LoadPluginFile(string loadFileName, bool stratPlugins)
        {
            string ext = Path.GetExtension(loadFileName).ToLower();
            if (ext == ".cs")
            {
                LoadCSharpScriptFile(loadFileName, stratPlugins);
            }
            else if (ext == ".dll" || ext == ".exe")
            {
                try
                {
                    LoadAssembly(loadFileName, stratPlugins);
                }
                catch (BadImageFormatException)
                {
                    // non .NET .dlls
                }
                catch (ReflectionTypeLoadException)
                {
                    // Out of date or dlls missing sub dependencies
                }
                catch (TypeLoadException)
                {
                    // Another version of: Out of date or dlls missing sub dependencies
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR in Radegast Plugin: " + loadFileName + " because " + ex, Helpers.LogLevel.Debug);
                }
            }
        }

        /// <summary>
        /// Scans and load plugins from Radegast application folder without starting them
        /// </summary>
        public void ScanAndLoadPlugins()
        {
            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(dirName)) return;

            foreach (string loadFileName in Directory.GetFiles(dirName))
            {
                if (IsUnusedFile(loadFileName))
                {
                    continue;
                }

                LoadPluginFile(loadFileName, false);
            }
        }

        private static bool IsUnusedFile(string loadFileName)
        {
            if (!AllowedPluginExtensions.Contains(Path.GetExtension(loadFileName).ToLower())) return true;
            loadFileName = Path.GetFileName(loadFileName).ToLower();

            foreach (string blackList in PluginBlackList)
            {
                if (loadFileName.StartsWith(blackList.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Loads and compiles a plugin from a C# source file
        /// </summary>
        /// <param name="fileName">Load plugin from this filename</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadCSharpScriptFile(string fileName, bool startPlugins)
        {
            try { LoadCSharpScript(fileName, File.ReadAllText(fileName), startPlugins); }
            catch (Exception ex)
            {
                Logger.Log("Failed loading C# script " + fileName + ": ", Helpers.LogLevel.Warning, ex);
            }
        }

        /// <summary>
        /// Compiles plugin from string source code
        /// </summary>
        /// <param name="fileName">File name from which source was loaded</param>
        /// <param name="code">Source code</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadCSharpScript(string fileName, string code, bool startPlugins)
        {
            try
            {
                // *** Generate dynamic compiler
                Dictionary<string, string> loCompilerOptions = new Dictionary<string, string>();
                loCompilerOptions.Add("CompilerVersion", "v3.5");
                CSharpCodeProvider loCompiler = new CSharpCodeProvider(loCompilerOptions);
                CompilerParameters loParameters = new CompilerParameters();

                // *** Start by adding any referenced assemblies
                loParameters.ReferencedAssemblies.Add("OpenMetaverse.StructuredData.dll");
                loParameters.ReferencedAssemblies.Add("OpenMetaverseTypes.dll");
                loParameters.ReferencedAssemblies.Add("OpenMetaverse.dll");
                loParameters.ReferencedAssemblies.Add("Radegast.exe");
                loParameters.ReferencedAssemblies.Add("System.dll");
                loParameters.ReferencedAssemblies.Add("System.Core.dll");
                loParameters.ReferencedAssemblies.Add("System.Xml.dll");
                loParameters.ReferencedAssemblies.Add("System.Drawing.dll");
                loParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

                // *** Load the resulting assembly into memory
                loParameters.GenerateInMemory = true;
                loParameters.GenerateExecutable = false;

                // *** Now compile the whole thing
                CompilerResults loCompiled =
                        loCompiler.CompileAssemblyFromSource(loParameters, code);

                // *** Check for compilation erros
                if (loCompiled.Errors.HasErrors)
                {
                    string lcErrorMsg = "";
                    lcErrorMsg = "Compilation failed: " + loCompiled.Errors.Count.ToString() + " errors:";

                    for (int x = 0; x < loCompiled.Errors.Count; x++)
                        lcErrorMsg += "\r\nLine: " +
                                     loCompiled.Errors[x].Line.ToString() + " - " +
                                     loCompiled.Errors[x].ErrorText;

                    instance.TabConsole.DisplayNotificationInChat(lcErrorMsg, ChatBufferTextStyle.Alert);
                    return;
                }

                instance.TabConsole.DisplayNotificationInChat("Compilation successful.");
                Assembly loAssembly = loCompiled.CompiledAssembly;
                LoadAssembly(fileName, loAssembly, startPlugins);
            }
            catch (Exception ex)
            {
                Logger.Log("Failed loading C# script: ", Helpers.LogLevel.Warning, ex);
            }
        }

        /// <summary>
        /// Scans assembly for supported types
        /// </summary>
        /// <param name="loadfilename">File name from which assembly was loaded</param>
        /// <param name="assembly">Assembly to scan for supported types</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadAssembly(string loadfilename, bool startPlugins)
        {
            LoadAssembly(loadfilename, null, startPlugins);
        }


        /// <summary>
        /// Scans assembly for supported types and loads it into it's own domain
        /// </summary>
        /// <param name="loadfilename">File name from which assembly was loaded</param>
        /// <param name="startPlugins">Start plugins found in the assembly after complilation</param>
        public void LoadAssembly(string loadfilename, Assembly assembly, bool startPlugins)
        {
            if (null != PluginsLoaded.Find((PluginInfo info) => { return info.FileName == loadfilename; }))
            {
                Logger.Log("Plugin already loaded, skipping: " + loadfilename, Helpers.LogLevel.Info);
                if (startPlugins)
                {
                    instance.TabConsole.DisplayNotificationInChat("Plugin already loaded, skipping: " + loadfilename);
                }
                return;
            }

            AppDomain domain = null;

            if (assembly == null)
            {
                // Don't load ourselves into a domain
                if (Path.GetFileName(Assembly.GetEntryAssembly().Location) == Path.GetFileName(loadfilename))
                {
                    assembly = Assembly.GetEntryAssembly();
                }
                else
                {
                    assembly = Assembly.LoadFile(loadfilename);

                    /* Disable creation of domains for now
                    domain = AppDomain.CreateDomain("Domain for: " + loadfilename);
                    var loader = (RemoteLoader)domain.CreateInstanceAndUnwrap("Radegast", "Radegast.RemoteLoader");
                    assembly = loader.Load(loadfilename);
                    */
                }
            }

            bool loadedTypesFromAssembly = false;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IRadegastPlugin).IsAssignableFrom(type))
                {
                    if (type.IsInterface) continue;
                    try
                    {
                        IRadegastPlugin plug = null;
                        ConstructorInfo constructorInfo = type.GetConstructor(new Type[] { typeof(RadegastInstance) });
                        if (constructorInfo != null)
                        {
                            plug = (IRadegastPlugin)constructorInfo.Invoke(new[] { instance });
                        }
                        else
                        {
                            constructorInfo = type.GetConstructor(new Type[] { });
                            if (constructorInfo != null)
                            {
                                plug = (IRadegastPlugin)constructorInfo.Invoke(new object[0]);
                            }
                            else
                            {
                                Logger.Log("ERROR Constructing Radegast Plugin: " + loadfilename + " because " + type + " has no usable constructor.", Helpers.LogLevel.Debug);
                                continue;
                            }
                        }

                        loadedTypesFromAssembly = true;

                        PluginInfo info = new PluginInfo()
                        {
                            FileName = loadfilename,
                            Plugin = plug,
                            Started = false,
                            Domain = domain
                        };

                        lock (PluginsLoaded) PluginsLoaded.Add(info);
                        if (startPlugins && plug != null)
                        {
                            try { plug.StartPlugin(instance); info.Started = true; }
                            catch (Exception ex) { Logger.Log(string.Format("Failed starting plugin {0}:", type), Helpers.LogLevel.Error, ex); }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR Constructing Radegast Plugin: " + loadfilename + " because " + ex,
                                   Helpers.LogLevel.Debug);
                    }
                }
                else
                {
                    try
                    {
                        loadedTypesFromAssembly |= instance.CommandsManager.LoadType(type);
                        loadedTypesFromAssembly |= instance.ContextActionManager.LoadType(type);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Radegast Plugin: " + loadfilename + " Command: " + type +
                                   " because " + ex.Message + " " + ex.StackTrace, Helpers.LogLevel.Debug);
                    }
                }
            }

            if (domain != null && !loadedTypesFromAssembly)
            {
                AppDomain.Unload(domain);
            }


        }
    }

    public class RemoteLoader : MarshalByRefObject
    {
        public Assembly Load(string loadfilename)
        {
            return AppDomain.CurrentDomain.Load(File.ReadAllBytes(loadfilename));
        }
    }

}
