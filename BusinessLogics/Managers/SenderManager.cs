using BusinessLogics.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace BusinessLogics.Managers
{
    public class SenderManager
    {
        [ImportMany(typeof(ISenderMessage))]

        public readonly Dictionary<string, ISenderMessage> Plugins = new Dictionary<string, ISenderMessage>();
        public List<string> Headers { get; set; }
        public SenderManager()
        {
            string PluginPath = "C:\\Users\\pavlo\\source\\repos\\COP\\Plugins";
            foreach (var dll in Directory.GetFiles(PluginPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(dll);
                try
                {
                    ISenderMessage plugin = Activator.CreateInstance(assembly.GetTypes()[0]) as ISenderMessage;
                    Plugins.Add(Path.GetFileNameWithoutExtension(dll), plugin);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }
    }
}
