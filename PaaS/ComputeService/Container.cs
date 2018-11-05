using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.ComputeService
{
    class Container
    {
        public int Id { get; private set; }
        public int Port { get; private set; }
        public string BaseDirectory { get; private set; }
        public string Endpoint { get; private set; }
        public Process Process { get; private set; }
        public ChannelFactory<IContainer> ChannelFactory { get; private set; }
        public IContainer Proxy { get; private set; }
        public bool IsFree { get; set; }
        public string LoadedAssemblyName { get; set; }

        public Container(int id, int port)
        {
            Id = id;
            Port = port;
            BaseDirectory = $@"{Config.Instance.ContainerDirectory}\{Id}";
            Endpoint = $"net.tcp://localhost:{Port}/{Config.Instance.ContainerEndpointPath}";
            StartProcess();
            GenerateProxy();
            IsFree = true;
            LoadedAssemblyName = string.Empty;
            CreateBaseDirectory();
        }

        private void CreateBaseDirectory()
        {
            if (Directory.Exists($@"{Config.Instance.ContainerDirectory}/{Id}"))
            {
                Directory.Delete($@"{Config.Instance.ContainerDirectory}/{Id}", true);
            }
            Directory.CreateDirectory($@"{Config.Instance.ContainerDirectory}/{Id}");
        }

        private void StartProcess()
        {
            Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Config.Instance.ContainerExecutablePath,
                    Arguments = $"{Id} {BaseDirectory} {Endpoint}",
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };
            Process.Start();
            Console.WriteLine($"Container{Id} ready");
        }

        private void GenerateProxy()
        {
            var binding = new NetTcpBinding();
            var address = new EndpointAddress(Endpoint);
            ChannelFactory = new ChannelFactory<IContainer>(binding, address);
            Proxy = ChannelFactory.CreateChannel();
        }

        public void LoadAssembly(string packageName, byte[] package)
        {
            try
            {
                Console.WriteLine($"Loading Container{Id} with {packageName}.dll . . .");
                IsFree = false;
                var path = $@"{Config.Instance.ContainerDirectory}\{Id}";
                Package.Save(path, packageName, package, Id.ToString());
                var assemblyName = $"{packageName}.dll";
                LoadedAssemblyName = packageName;
                var result = Proxy.Load(assemblyName);
                Console.WriteLine(result);
            }
            catch (CommunicationException) { }
        }

        public string CheckState()
        {
            string result = null;
            try
            {
                result = Proxy.CheckState();
            }
            catch (CommunicationException)
            {
                result = "Error";
            }
            return result;
        }

        public void Dispose()
        {
            if (Directory.Exists($@"{Config.Instance.ContainerDirectory}/{Id}"))
            {
                Directory.Delete($@"{Config.Instance.ContainerDirectory}/{Id}", true);
            }
        }

        public void Kill()
        {
            if (!Process.HasExited)
            {
                Process.Kill();
            }
        }
    }
}
