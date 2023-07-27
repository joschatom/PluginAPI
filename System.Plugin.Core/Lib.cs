using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.SymbolStore;
using System.Plugin.Resource;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace System.Plugin.Resource
{
    public interface IResource : IDisposable {
        void Initialize();
    };
}

namespace System.Plugin.Core
{
   

    [Serializable]
    public class Plugin : IDisposable
    {

        public string Name { get; internal set; } = "";
        public Guid Identifier { get; internal set; } = Guid.Empty;

        private List<(Type @Type, IResource @Value)> _Resources = new();
        protected internal List<(Type @Type, IResource @Value)> Resources {
            get
            {
                return _Resources;
            }
            protected set 
            {
                _Resources = Resources.AsEnumerable().Concat(value.AsEnumerable()).ToList();
            }
        }

        public Plugin(string? name, Guid identifier)
        {
            Name = name ?? "Unnamed";
            Identifier = identifier;
        }

        public void Dispose()
        {
            Resources.ForEach((resource) => {
                resource.Value.Dispose();
            });

            GC.SuppressFinalize(this);
        }
    }
}