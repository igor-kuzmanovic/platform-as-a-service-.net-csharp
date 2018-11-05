using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.SharedLibrary
{
    [ServiceContract]
    public interface IContainer
    {
        [OperationContract]
        string Load(string assemblyName);

        [OperationContract]
        String CheckState();
    }
}
