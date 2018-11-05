using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaaS.Worker2
{
    [Export(typeof(IWorkerRole))]
    public class Worker2 : IWorkerRole
    {
        public void Start(string containerId)
        {
            var proxy = GenerateProxy();
            var myAssemblyName = typeof(Worker2).Name;
            var myAddress = proxy.GetAddress(myAssemblyName, containerId);
            Console.WriteLine($"Worker2 started on Container{containerId}, address: {myAddress}");

            do
            {
                try
                {
                    var brotherInstanceAddresses = proxy.BrotherInstances(myAssemblyName, myAddress);
                    if (brotherInstanceAddresses != null && brotherInstanceAddresses.Any())
                    {
                        Console.WriteLine("Brother instance addresses:");
                        foreach (var address in brotherInstanceAddresses)
                        {
                            Console.WriteLine(address);
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no brother instances");
                    }
                    Console.WriteLine("Press any key to continue or ESC to exit . . .");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.TargetSite.ReflectedType.Name}.{e.TargetSite.Name}: {e.Message}");
                    Stop();
                    break;
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            Console.WriteLine("Worker2 finished");
        }

        public void Stop()
        {
            Console.WriteLine("Worker2 stopped");
        }

        private static IRoleEnvironment GenerateProxy()
        {
            var binding = new NetTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:10010/RoleEnvironment");
            var channelFactory = new ChannelFactory<IRoleEnvironment>(binding, address);
            return channelFactory.CreateChannel();
        }
    }
}
