using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PaaS.ComputeService
{
    class Config
    {
        public static Config Instance { get; } = new Config();

        public string ContainerExecutablePath { get; private set; }
        public string ContainerDirectory { get; private set; }
        public string ContainerEndpointPath { get; private set; }
        public int ContainerCapacity { get; private set; }
        public string PackageDirectory { get; private set; }
        public int PackageCheckInterval { get; private set; }
        public int StateCheckInterval { get; private set; }
        public string RoleEnvironmentEndpointPath { get; private set; }
        public int RoleEnvironmentEndpointPort { get; private set; }
        public int MinPort { get; private set; }
        public int MaxPort { get; private set; }

        private Config()
        {
            XElement xml = XElement.Load("ComputeService.xml");
            ContainerExecutablePath = xml.Element("ContainerExecutablePath").Value;
            ContainerDirectory = xml.Element("ContainerDirectory").Value;
            ContainerEndpointPath = xml.Element("ContainerEndpointPath").Value;
            ContainerCapacity = Int32.Parse(xml.Element("ContainerCapacity").Value);
            PackageDirectory = xml.Element("PackageDirectory").Value;
            PackageCheckInterval = Int32.Parse(xml.Element("PackageCheckInterval").Value);
            StateCheckInterval = Int32.Parse(xml.Element("StateCheckInterval").Value);
            RoleEnvironmentEndpointPath = xml.Element("RoleEnvironmentEndpointPath").Value;
            RoleEnvironmentEndpointPort = Int32.Parse(xml.Element("RoleEnvironmentEndpointPort").Value);
            MinPort = Int32.Parse(xml.Element("MinPort").Value);
            MaxPort = Int32.Parse(xml.Element("MaxPort").Value);
        }
    }
}
