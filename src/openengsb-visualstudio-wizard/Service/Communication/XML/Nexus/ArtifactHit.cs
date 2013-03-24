using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication.XML.Nexus
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
