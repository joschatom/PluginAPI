using System.Reflection;
using System.Plugin;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.ComponentModel;
using System.Collections;
using static System.Plugin.Core.Plugin;

namespace System.Plugin
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ExportPluginFunction : Attribute {}

    public sealed class PluginHost
    {
        public Dictionary<Guid, Assembly> Assemblies { get; private set; } = new Dictionary<Guid, Assembly>();
        public Dictionary<Guid, (Type @Type, object Instance)> Plugins { get; private set; } = new Dictionary<Guid, (Type, object)>();
        public Hashtable PluginFunctions { get; private set; } = new Hashtable();

        public Guid Identifier { get; init; }

        public PluginHost(Guid identifier) { Identifier = identifier; }

        public Guid LoadPlugin(string dllPath)
        {
            Assembly assembly = Assembly.LoadFile(dllPath);

            Type basePlugin = typeof(Core.Plugin);

            IEnumerable<Type> derivedTypes = assembly.GetTypes().Where(t => basePlugin.IsAssignableFrom(t) && t != basePlugin);
            if (derivedTypes.Count() > 1) throw new Exception($"{dllPath} contains multiple Plugins, but right now only one plugin per DLL is allowed!");

            Guid guid = Guid.NewGuid();

            var plugin = Tuple.Create<Type, object>(derivedTypes.First(), (Activator.CreateInstance(derivedTypes.First(), [null, guid]) ?? throw new Exception("Failed to create Plugin instance using Activator!")));

            Assemblies.Add(guid, assembly);
            Plugins.Add(guid, plugin.ToValueTuple());

            (plugin.Item2 as Core.Plugin)!._HostFunction += (sender, func, args) =>
            {
                return PluginFunctions[func] is MethodBase method ? method.Invoke(sender, args) : null;
            };

            return guid;
        }

        public void UseAssemblyFunctions(Assembly assembly)
        {
            assembly.GetTypes().ToList().ForEach((@type) =>
            {
                @type.GetMethods().ToList().ForEach((method) =>
                {
                    method.GetCustomAttributes(false).ToList().ForEach((attribute) =>
                    {
                        if (attribute is ExportPluginFunction exportFunctionTo)
                        {
                            PluginFunctions.Add(method.Name, method);
                            
                        }
                    });
                });
            });
        }

        public void UnloadPlugin(Guid guid)
        {
            (Plugins[guid].Instance as Core.Plugin)!.Dispose();
            Plugins.Remove(guid);
            Assemblies.Remove(guid);
        }

        public void UnloadAllPlugins()
        {
            Plugins.Keys.ToList().ForEach((guid) =>
            {
                UnloadPlugin(guid);
            });
        }

        
    }
}