using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.Common;
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

        public delegate object? HostFunctionCallEvent(object? sender, string name, object?[]? args);

        public string Name { get; internal set; } = "";
        public Guid Identifier { get; internal set; } = Guid.Empty;

        public event HostFunctionCallEvent _HostFunction;

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

        public (Type @Type, object? @Value)? TryCall(string func, bool failOnNotFound = false, params object?[]? args)
        {
            Type Self = this.GetType();

            var arg_types =  args?.Select((arg) => arg?.GetType() ?? typeof(object)).ToArray() ?? new Type[0];

            var method = Self.GetRuntimeMethod(func, arg_types);
            if (method == null) if (failOnNotFound) throw new Exception($"Failed to find method {func} with arguments {arg_types}!");
                else return null;

            return (method.ReturnType, method.Invoke(this, args));
        }

        public object? CallHost(string func, params object?[]? args)
        {
            return _HostFunction?.Invoke(this, func, args);
        }
    }
}