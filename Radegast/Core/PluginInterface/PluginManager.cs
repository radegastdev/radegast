// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using OpenMetaverse;
using Microsoft.CSharp;

namespace Radegast
{
    /// <summary>
    /// Handles loading Radegast plugins
    /// </summary>
    public class PluginManager : IDisposable
    {
        List<IRadegastPlugin> PluginsLoaded = new List<IRadegastPlugin>();
        RadegastInstance instance;

        public List<IRadegastPlugin> Plugins
        {
            get
            {
                List<IRadegastPlugin> ret = null;
                lock (PluginsLoaded)
                    ret = new List<IRadegastPlugin>(PluginsLoaded);
                return ret;
            }
        }

        public PluginManager(RadegastInstance instance)
        {
            this.instance = instance;
        }

        public void Dispose()
        {
            lock (PluginsLoaded)
            {
                List<IRadegastPlugin> unload = new List<IRadegastPlugin>(PluginsLoaded);
                unload.ForEach(plug =>
                {
                    PluginsLoaded.Remove(plug);
                    try
                    {
                        plug.StopPlugin(instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Shutdown Plugin: " + plug + " because " + ex, Helpers.LogLevel.Debug, ex);
                    }
                });
            }
        }

        public static PluginAttribute GetAttributes(IRadegastPlugin plug)
        {
            PluginAttribute a = null;

            foreach(Attribute attr in Attribute.GetCustomAttributes(plug.GetType()))
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

        public void StartPlugins()
        {
            lock (PluginsLoaded)
            {
                foreach (IRadegastPlugin plug in PluginsLoaded)
                {
                    try
                    {
                        plug.StartPlugin(instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Starting Radegast Plugin: " + plug + " because " + ex, Helpers.LogLevel.Debug);
                    }
                }
            }
        }

        public void ScanAndLoadPlugins()
        {
            string dirName = Application.StartupPath;

            if (!Directory.Exists(dirName)) return;

            foreach (string loadfilename in Directory.GetFiles(dirName))
            {
                if (loadfilename.ToLower().EndsWith(".dll") || loadfilename.ToLower().EndsWith(".exe"))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(loadfilename);
                        LoadAssembly(loadfilename, assembly);
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
                        Logger.Log("ERROR in Radegast Plugin: " + loadfilename + " because " + ex, Helpers.LogLevel.Debug);
                    }
                }
            }
        }

        public void LoadCSharpScriptFile(string filename)
        {
            try { LoadCSharpScript(File.ReadAllText(filename)); }
            catch (Exception ex)
            {
                Logger.Log("Failed loading C# script " + filename + ": ", Helpers.LogLevel.Warning, ex);
            }
        }

        public void LoadCSharpScript(string code)
        {
            ThreadPool.QueueUserWorkItem(sender =>
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
                    instance.MainForm.Invoke(new MethodInvoker(() => LoadAssembly("Dynamically compiled", loAssembly, true)));
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed loading C# script: ", Helpers.LogLevel.Warning, ex);
                }
            });
        }

        public void LoadAssembly(string loadfilename, Assembly assembly)
        {
            LoadAssembly(loadfilename, assembly, false);
        }

        public void LoadAssembly(string loadfilename, Assembly assembly, bool startPlugins)
        {
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
                            plug = (IRadegastPlugin)constructorInfo.Invoke(new[] { instance });
                        else
                        {
                            constructorInfo = type.GetConstructor(new Type[] { });
                            if (constructorInfo != null)
                                plug = (IRadegastPlugin)constructorInfo.Invoke(new object[0]);
                            else
                            {
                                Logger.Log("ERROR Constructing Radegast Plugin: " + loadfilename + " because " + type + " has no usable constructor.", Helpers.LogLevel.Debug);
                                continue;
                            }
                        }
                        lock (PluginsLoaded) PluginsLoaded.Add(plug);
                        if (startPlugins && plug != null)
                        {
                            try { plug.StartPlugin(instance); }
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
                        instance.CommandsManager.LoadType(type);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Radegast Plugin: " + loadfilename + " Command: " + type +
                                   " because " + ex.Message + " " + ex.StackTrace, Helpers.LogLevel.Debug);
                    }
                }
            }
        }
    }
}
