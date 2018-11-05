using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaaS.ComputeService
{
    static class ContainerManager
    {
        public static int Capacity { get; private set; } = Config.Instance.ContainerCapacity;
        public static List<Container> Containers { get; private set; } = new List<Container>(Capacity);

        public static void GenerateContainers()
        {
            Console.WriteLine("Generating containers . . .");
            CreateBaseDirectory();
            for (int i = 0; i < Capacity; i++)
            {
                GenerateContainer();
            }
        }

        public static void LoadOnePackage()
        {
            var path = Config.Instance.PackageDirectory;
            var packageName = Package.GetOne(path, PackageType.WithConfig);
            if (packageName == string.Empty)
                return;

            var freeContainerCount = GetAvailableCount();
            var instanceCount = Package.GetInstanceCount(path, packageName);
            if (instanceCount <= 0 || instanceCount > freeContainerCount)
            {
                if (instanceCount <= 0)
                    Console.WriteLine($"Invalid number of instances for {packageName}.dll");
                else
                    Console.WriteLine($"Not enough available containers for {packageName}.dll");
                Package.Delete(path, packageName);
                return;
            }

            for (int i = 0; i < instanceCount; i++)
            {
                var container = GetOneAvailable();
                var package = Package.Load(path, packageName);
                container.LoadAssembly(packageName, package);
            }
            Package.Delete(path, packageName);
        }

        public static void CheckStateAll()
        {
            foreach (var container in Containers)
            {
                string result = container.CheckState();
                if (result.Equals("Error"))
                {
                    Console.WriteLine($"Container{container.Id} offline, starting recovery . . .");
                    RecoverOne(container);
                    break;
                }
                else if (result.Equals("Free"))
                {
                    container.IsFree = true;
                    container.LoadedAssemblyName = string.Empty;
                }
            }
        }

        private static void RecoverOne(Container container)
        {
            if (!container.IsFree)
            {
                if (GetAvailableCount() > 0)
                {
                    var freeContainer = GetOneAvailable();
                    var path = container.BaseDirectory;
                    var packageName = Package.GetOne(path, PackageType.WithoutConfig);
                    var package = Package.Load(path, packageName);
                    freeContainer.LoadAssembly(packageName, package);
                    GenerateContainer();
                }
                else
                {
                    var newContainer = GenerateContainer();
                    var path = container.BaseDirectory;
                    var packageName = Package.GetOne(path, PackageType.WithoutConfig);
                    var package = Package.Load(path, packageName);
                    newContainer.LoadAssembly(packageName, package);
                }
            }
            else
            {
                GenerateContainer();
            }
            container.Dispose();
            Containers.Remove(container);
        }

        private static Container GenerateContainer()
        {
            var id = GetFreeId();
            var port = GetFreePort();
            var container = new Container(id, port);
            Containers.Add(container);
            Console.WriteLine($"Generated Container{id}, dedicated port: {port}");
            return container;
        }

        private static void CreateBaseDirectory()
        {
            if (Directory.Exists($@"{Config.Instance.ContainerDirectory}"))
            {
                Directory.Delete($@"{Config.Instance.ContainerDirectory}", true);
            }
            Directory.CreateDirectory($@"{Config.Instance.ContainerDirectory}");
        }

        private static int GetAvailableCount()
        {
            return Containers.Count(c => c.IsFree == true);
        }

        private static Container GetOneAvailable()
        {
            return Containers.FirstOrDefault(c => c.IsFree == true);
        }

        private static int GetFreeId()
        {
            var containerIds = Containers.Select(c => c.Id).ToArray();
            for (int id = 0; id < Int32.MaxValue; id++)
            {
                if (!containerIds.Contains(id))
                {
                    return id;
                }
            }
            return -1;
        }

        private static int GetFreePort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endpoints = properties.GetActiveTcpListeners();
            var usedPorts = endpoints.Select(p => p.Port).ToList();
            var containerPorts = Containers.Select(c => c.Port).ToArray();
            for (int port = Config.Instance.MinPort; port <= Config.Instance.MaxPort; port++)
            {
                if (!usedPorts.Contains(port) && !containerPorts.Contains(port))
                {
                    return port;
                }
            }
            return -1;
        }

        public static void KillAll()
        {
            foreach (var container in Containers)
            {
                container.Dispose();
                container.Kill();
            }
        }
    }
}
