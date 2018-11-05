using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.Container
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Initialize(args);

                do
                {
                    while (!Console.KeyAvailable);
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.TargetSite.ReflectedType.Name}.{e.TargetSite.Name}: {e.Message}");
            }
            finally
            {
                Connection.CloseContainerEndpoint();
                Console.WriteLine("Press any key to exit . . .");
                Console.ReadKey(true);
            }
        }

        static void Initialize(string[] args)
        {
            if (args.Length == 3)
            {
                int id = Int32.Parse(args[0]);
                string baseDirectory = args[1];
                string endpoint = args[2];
                Container container = new Container(id, baseDirectory);
                Connection.OpenContainerEndpoint(container, endpoint);
                Console.WriteLine($"Container{container.Id} ready");
            }
        }
    }
}
