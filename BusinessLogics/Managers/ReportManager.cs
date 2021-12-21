using BusinessLogics.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace BusinessLogics.Managers
{
    public class ReportManager
    {
        [ImportMany(typeof(IReportBuilder))]
        public readonly Dictionary<string, IReportBuilder> Plugins = new Dictionary<string, IReportBuilder>();
        public List<string> Headers { get; set; }
        public ReportManager()
        {
            string PluginPath = "C:\\Users\\pavlo\\source\\repos\\COP\\Plugins";
            foreach (var dll in Directory.GetFiles(PluginPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(dll);
                try
                {
                    IReportBuilder plugin = Activator.CreateInstance(assembly.GetTypes()[0]) as IReportBuilder;
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
