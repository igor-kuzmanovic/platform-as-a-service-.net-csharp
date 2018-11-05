using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.ComputeService
{
    static class Connection
    {
        private static ServiceHost ServiceHost { get; set; }
        private static int Port { get; set; } = Config.Instance.RoleEnvironmentEndpointPort;
        private static string RoleEnvironmentEndpointPath { get; set; } = Config.Instance.RoleEnvironmentEndpointPath;
        public static string RoleEnvironmentEndpoint { get; private set; } = $"net.tcp://localhost:{Port}/{RoleEnvironmentEndpointPath}";

        public static void OpenRoleEnvironmentEndpoint()
        {
            if (ServiceHost == null)
            {
                var binding = new NetTcpBinding();
                var address = new Uri(RoleEnvironmentEndpoint);
                ServiceHost = new ServiceHost(typeof(RoleEnvironment));
                ServiceHost.AddServiceEndpoint(typeof(IRoleEnvironment), binding, address);
                ServiceHost.Open();
            }
        }

        public static void CloseRoleEnvironmentEndpoint()
        {
            if (ServiceHost.State == CommunicationState.Opened)
                ServiceHost.Close();
        }
    }
}
