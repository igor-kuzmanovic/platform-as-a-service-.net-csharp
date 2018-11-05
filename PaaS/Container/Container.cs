using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.Container
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class Container : IContainer
    {
        public int Id { get; private set; }
        public string BaseDirectory { get; private set; }
        private string State { get; set; }

        public Container(int id, string baseDirectory)
        {
            Id = id;
            BaseDirectory = baseDirectory;
            State = "Free";
        }

        public string Load(string assemblyName)
        {
            try
            {
                State = "Busy";
                Console.WriteLine($"Loading {assemblyName} . . .");
                var assemblyPath = $@"{BaseDirectory}\{assemblyName}";
                var assemblyBytes = File.ReadAllBytes(assemblyPath);
                var assembly = Assembly.Load(assemblyBytes);
                Component.Instance.AddComponent(assembly);
                Task.Run(() =>
                {
                    Component.Instance.WorkerRole.Start(Id.ToString());
                    Component.Instance.Reset();
                    if (File.Exists(assemblyPath))
                        File.Delete(assemblyPath);
                    State = "Free";
                });
                return $"Container{Id} - {assemblyName} successfully started";
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.TargetSite.ReflectedType.Name}.{e.TargetSite.Name}: {e.Message}");
                return $"Container{Id} - {e.TargetSite.ReflectedType.Name}.{e.TargetSite.Name}: {e.Message}";
            }
        }

        public string CheckState()
        {
            return State;
        }
    }
}
