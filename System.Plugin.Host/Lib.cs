using System.Reflection;
using System.Plugin;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.ComponentModel;

namespace System.Plugin
{
    public class PluginHost
    {
        public Dictionary<Guid, Assembly> Assemblies { get; private set; } = new Dictionary<Guid, Assembly>();
        public Dictionary<Guid, (Type @Type, object Instance)> Plugins { get; private set; } = new Dictionary<Guid, (Type, object)>();

        public PluginHost() { }

        public void LoadPlugin(string dllPath)
        {
            Assembly assembly = Assembly.Load(dllPath);


            Type basePlugin = typeof(Core.Plugin);

            IEnumerable<Type> derivedTypes = assembly.GetTypes().Where(t => basePlugin.IsAssignableFrom(t) && t != basePlugin);
            if (derivedTypes.Count() > 1) throw new Exception($"{dllPath} contains multiple Plugins, but right now only one plugin per DLL is allowed!");

            Guid guid = Guid.NewGuid();

            var plugin = Tuple.Create<Type, object>(derivedTypes.First(), (Activator.CreateInstance(derivedTypes.First(), [null, guid]) ?? throw new Exception("Failed to create Plugin instance using Activator!")));

            Assemblies.Add(guid, assembly);
            Plugins.Add(guid, plugin.ToValueTuple());
        }
    }
}