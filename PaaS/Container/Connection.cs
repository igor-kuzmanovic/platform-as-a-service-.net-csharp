using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.Container
{
    static class Connection
    {
        private static ServiceHost ServiceHost { get; set; }

        public static void OpenContainerEndpoint(Container container, string endpoint)
        {
            var binding = new NetTcpBinding();
            var address = new Uri(endpoint);
            ServiceHost = new ServiceHost(container);
            ServiceHost.AddServiceEndpoint(typeof(IContainer), binding, address);
            ServiceHost.Open();
        }

        public static void CloseContainerEndpoint()
        {
            if (ServiceHost.State == CommunicationState.Opened)
                ServiceHost.Close();
        }
    }
}
