using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PaaS.ComputeService
{
    class Package
    {
        public static string GetOne(string path, PackageType packageType)
        {
            if (Directory.Exists(path))
            {
                var assemblyPaths = Directory.GetFiles(path, "*.dll");
                var noExtAssemblyPaths = assemblyPaths.Select(s => s.Remove(s.Length - 4));
                IEnumerable<string> noExtPaths = null;
                if (packageType == PackageType.WithConfig)
                {
                    var configPaths = Directory.GetFiles(path, "*.xml");
                    var noExtConfigPaths = configPaths.Select(s => s.Remove(s.Length - 4));
                    noExtPaths = noExtAssemblyPaths.Intersect(noExtConfigPaths);
                }
                else if (packageType == PackageType.WithoutConfig)
                {
                    noExtPaths = noExtAssemblyPaths;
                }
                if (noExtPaths.Any())
                {
                    return noExtPaths.First().Split('\\').Last();
                }
            }
            return string.Empty;
        }

        public static int GetInstanceCount(string path, string packageName)
        {
            path += $@"\{packageName}.xml";
            if (File.Exists(path))
            {
                var xml = XElement.Load(path);
                var instanceCount = Int32.Parse(xml.Element("InstanceCount").Value);
                Console.WriteLine($"Requesting {instanceCount} instances of {packageName}.dll . . .");
                return instanceCount;
            }
            Console.WriteLine($"Config for {packageName}.dll not found");
            return 0;
        }

        public static void Save(string path, string packageName, byte[] package, string containerId)
        {
            path += $@"\{packageName}.dll";
            if (!File.Exists(path))
                File.WriteAllBytes(path, package);
        }

        public static byte[] Load(string path, string packageName)
        {
            path += $@"\{packageName}.dll";
            if (File.Exists(path))
                return File.ReadAllBytes(path);
            return null;
        }

        public static void Delete(string path, string packageName)
        {
            var dllPath = path + $@"\{packageName}.dll";
            if (File.Exists(dllPath))
                File.Delete(dllPath);
            var xmlPath = path + $@"\{packageName}.xml";
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
        }
    }
}
