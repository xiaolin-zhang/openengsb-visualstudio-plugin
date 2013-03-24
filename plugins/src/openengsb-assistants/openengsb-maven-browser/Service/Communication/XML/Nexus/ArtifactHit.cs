using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication.XML.Nexus
{
    [Serializable]
    [XmlType("artifactHit")]
    public class ArtifactHit
    {
        [XmlElement("repositoryId")]
        public string RepoId { get; set; }

        [XmlArray("artifactLinks")]
        public ArtifactLink[] Links { get; set; }

        public ArtifactHit()
        {
            this.RepoId = "";
            this.Links = null;
        }
    }
}
