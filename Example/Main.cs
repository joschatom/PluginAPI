using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Plugin;
using System.Plugin.Core;

namespace Example
{
    public class Example_Main
    {
        public static void Main()
        {
            // test
            PluginHost pluginHost = new PluginHost();

            pluginHost.LoadPlugin(@"C:\Users\Joscha\source\repos\System.Plugin\Example.Plugin\bin\Debug\net6.0-windows\Example.Plugin.dll");
        }
    }
}
