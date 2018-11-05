using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.ComputeService
{
    public class RoleEnvironment : IRoleEnvironment
    {
        public string[] BrotherInstances(string myAssemblyName, string myAddress)
        {
            var containers = ContainerManager.Containers.Where(c => c.LoadedAssemblyName == myAssemblyName && $"net.tcp://localhost:{c.Port}" != myAddress);
            return !containers.Any() ? new string[0] : containers.Select(c => $"net.tcp://localhost:{c.Port}").ToArray();
        }

        public string GetAddress(string myAssemblyName, string containerId)
        {
            var container = ContainerManager.Containers.FirstOrDefault(c => c.LoadedAssemblyName == myAssemblyName && c.Id.ToString() == containerId);
            return container == default(Container) ? string.Empty : $"net.tcp://localhost:{container.Port}";
        }
    }
}
