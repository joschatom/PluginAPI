using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Plugin;
using System.Plugin.Core;
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;
using System.Runtime.CompilerServices;

namespace Example
{



    public class Example_Main
    {

        [ExportPluginFunction]
        static public void Test0()
        {
            Console.WriteLine("Hello World!");
        }

        [ExportPluginFunction]
        static public void Test2()
        {
            Console.WriteLine("Hello World!");
        }

        public static void Main()
        {
            // test
            PluginHost pluginHost = new(new Guid("{31446C7E-DFC2-4962-8ACB-92800C3FA62C}"));

            pluginHost.LoadPlugin(@"C:\Users\Joscha\source\repos\System.Plugin\Example.Plugin\bin\Debug\net6.0-windows\Example.Plugin.dll");


            pluginHost.UseAssemblyFunctions(Assembly.GetExecutingAssembly());

            Console.WriteLine((pluginHost.Plugins.First().Value.Instance as Plugin)!.TryCall("Start"));

      
        }

    }

}
