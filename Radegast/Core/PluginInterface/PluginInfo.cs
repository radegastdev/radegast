using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radegast
{
    /// <summary>
    /// Information about loaded plugin
    /// </summary>
    public class PluginInfo
    {
        /// <summary>File name from which the plugin was loaded, cannot load plugin twice from the same file</summary>
        public string FileName { get; }

        /// <summary>Plugin class</summary>
        public IRadegastPlugin Plugin { get; }

        /// <summary>Is plugin started</summary>
        public bool IsStarted { get; private set; }

        /// <summary>Plugin class</summary>
        public PluginAttribute Attribures { get; }

        /// <summary>Domain the plugin was loaded in.</summary>
        public AppDomain Domain { get; }

        public PluginInfo(string filename, IRadegastPlugin plugin, AppDomain domain)
        {
            this.Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
            this.FileName = filename;
            this.Domain = domain;

            PluginAttribute pluginAttributes = null;
            try
            {
                var customAttributes = Attribute.GetCustomAttributes(Plugin.GetType());
                pluginAttributes = customAttributes.First(attribute => attribute is PluginAttribute) as PluginAttribute;
            }
            catch
            {
                // Suppress
            }

            this.Attribures = pluginAttributes ?? new PluginAttribute {Name = Plugin.GetType().FullName};
        }

        /// <summary>
        /// Starts the plugin. Attempts to start a plugin that has already been started will result in an exception.
        /// </summary>
        /// <param name="instance">Radegast instance</param>
        /// <exception cref="Exception">On failure</exception>
        public void Start(RadegastInstance instance)
        {
            if (!IsStarted)
            {
                Plugin.StartPlugin(instance);
                IsStarted = true;
            }
            else
            {
                throw new Exception("Plugin is already started");
            }
        }

        /// <summary>
        /// Stops the plugin. Attempts to stop a plugin that has already been stopped will result in an exception.
        /// </summary>
        /// <param name="instance">Radegast instance</param>
        /// <exception cref="Exception">On failure</exception>
        public void Stop(RadegastInstance instance)
        {
            if (IsStarted)
            {
                Plugin.StopPlugin(instance);
                IsStarted = false;
            }
            else
            {
                throw new Exception("Attemping is already stopped");
            }
        }
    }
}
