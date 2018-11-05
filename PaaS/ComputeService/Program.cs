using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PaaS.ComputeService
{
    class Program
    {
        private static int timer = 0;

        static void Main(string[] args)
        {
            try
            {
                Connection.OpenRoleEnvironmentEndpoint();
                ContainerManager.GenerateContainers();

                do
                {
                    do
                    {
                        if (timer != 0 && timer % Config.Instance.StateCheckInterval == 0)
                            ContainerManager.CheckStateAll();
                        if (timer != 0 && timer % Config.Instance.PackageCheckInterval == 0)
                            ContainerManager.LoadOnePackage();
                        if (timer < Int32.MaxValue)
                            timer++;
                        else
                            timer = 0;

                        Thread.Sleep(1);
                    }
                    while (!Console.KeyAvailable);
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.TargetSite.ReflectedType.Name}.{e.TargetSite.Name}: {e.Message}");
            }
            finally
            {
                Connection.CloseRoleEnvironmentEndpoint();
                ContainerManager.KillAll();
                Console.WriteLine("Press any key to exit . . .");
                Console.ReadKey(true);
            }
        }
    }
}
