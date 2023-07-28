using System.Linq.Expressions;
using System.Plugin.Core;
 

namespace Example
{
    public class ExamplePlugin : Plugin
    {
        public ExamplePlugin(string? name, Guid identifier) : base(name, identifier)
        {
           
        }

        public void Start()
        {
            CallHost("Test2");

            
        }
     
    }
}