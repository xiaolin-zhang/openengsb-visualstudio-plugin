using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;
using System.IO;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication
{
    public class LocalRepositoryClient : IRepositoryClient
    {
        private string _rootDir;
        public Filter ItemFilter { get; set; }

        public LocalRepositoryClient(string location)
        {
            _rootDir = location;
            ItemFilter = (string file) => file.EndsWith("DomainEvents.wsdl") || file.EndsWith("Domain.wsdl");
        }

        public IList<Artifact> GetArtifacts()
        {
            IList<Artifact> artifacts = new List<Artifact>();
            IList<string> files = listFiles(_rootDir);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                Common.CreateArtifactsFromFileName(artifacts, file, name);
            }
            return artifacts;
        }

        private IList<string> listFiles(string root)
        {
            List<string> files = new List<string>();
            foreach (string file in Directory.GetFiles(root))
            {
                if(ItemFilter(file))
                    files.Add(file);
            }
            foreach (string directory in Directory.GetDirectories(root))
            {
                files.AddRange(listFiles(directory));
            }
            return files;
        }
    }
}
