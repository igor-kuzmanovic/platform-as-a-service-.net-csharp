using PaaS.SharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PaaS.Container
{
    class Component
    {
        public static Component Instance { get; private set; } = new Component();

        [Import(typeof(IWorkerRole), AllowRecomposition = true)]
        public IWorkerRole WorkerRole { get; private set; }
        private CompositionContainer CompositionContainer { get; set; }

        private Component() { }

        public void AddComponent(Assembly assembly)
        {
            Instance.Reset();
            var catalog = new AssemblyCatalog(assembly);
            Instance.CompositionContainer = new CompositionContainer(catalog);
            Instance.CompositionContainer.ComposeParts(Instance);
        }

        public void Reset()
        {
            if (Instance.WorkerRole != null)
                CompositionContainer.ReleaseExport(new Export("IWorkerRole", () => Instance.WorkerRole));
        }
    }
}
