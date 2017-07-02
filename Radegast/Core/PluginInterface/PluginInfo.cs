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
        public PluginAttribute Attribures => GetAttributes();

        /// <summary>Domain the plugin was loaded in.</summary>
        public AppDomain Domain { get; }

        public PluginInfo(string filename, IRadegastPlugin plugin, AppDomain domain)
        {
            this.FileName = filename;
            this.Plugin = plugin;
            this.Domain = domain;
        }

        /// <summary>
        /// Gets extended atributes for plugin
        /// </summary>
        /// <returns>Extended atributes for plugin</returns>
        public PluginAttribute GetAttributes()
        {
            if (Plugin == null)
            {
                return null;
            }

            var customAttributes = Attribute.GetCustomAttributes(Plugin.GetType());
            var pluginAttributes = customAttributes.First(attribute => attribute is PluginAttribute) as PluginAttribute;

            return pluginAttributes ?? new PluginAttribute { Name = Plugin.GetType().FullName };
        }

        /// <summary>
        /// Starts the plugin.
        /// </summary>
        /// <param name="instance">Radegast instance</param>
        public void Start(RadegastInstance instance)
        {
            Plugin.StartPlugin(instance);
            IsStarted = true;
        }
    }
}
