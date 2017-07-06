using System;
using System.Linq;

namespace Radegast
{
    /// <summary>
    /// Information about loaded plugin
    /// </summary>
    public class PluginInfo
    {
        /// <summary>File name from which the plugin was loaded, cannot load plugin twice from the same file</summary>
        public string FileName { get; }

        /// <summary>Display name</summary>
        public string DisplayName { get; set; }

        /// <summary>Is plugin started</summary>
        public bool IsStarted { get; private set; }

        /// <summary>Plugin attributes</summary>
        public PluginAttribute Attribures { get; }

        /// <summary>Plugin instance</summary>
        private IRadegastPlugin PluginInstance { get; }

        public override string ToString() => DisplayName;

        /// <summary>
        /// Constructs a new PluginInfo
        /// </summary>
        /// <param name="filename">Path to the plugin.</param>
        /// <param name="plugin">Valid plugin instance. Must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="plugin"/> is <see langword="null"/></exception>
        public PluginInfo(string filename, IRadegastPlugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            this.PluginInstance = plugin;
            this.FileName = filename;
            this.DisplayName = PluginInstance.GetType().Name;

            PluginAttribute pluginAttributes = null;
            try
            {
                var customAttributes = Attribute.GetCustomAttributes(PluginInstance.GetType());
                pluginAttributes = customAttributes.First(attribute => attribute is PluginAttribute) as PluginAttribute;
            }
            catch
            {
                // Suppress
            }

            this.Attribures = pluginAttributes ?? new PluginAttribute {Name = PluginInstance.GetType().FullName};
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
                PluginInstance.StartPlugin(instance);
                IsStarted = true;
            }
            else
            {
                throw new Exception("Attempting to start a plugin that is already in the running state.");
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
                PluginInstance.StopPlugin(instance);
                IsStarted = false;
            }
            else
            {
                throw new Exception("Attempting to stop a plugin that is already in the stopped state.");
            }
        }
    }
}
