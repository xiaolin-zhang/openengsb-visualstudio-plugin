using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication.XML.Nexus
{
    [Serializable]
    [XmlType("artifact")]
    public class NexusArtifact
    {
        [XmlElement("groupId")]
        public string GroupId { get; set; }

        [XmlElement("artifactId")]
        public string ArtifactId { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("latestRelease")]
        public string LatestRelease { get; set; }

        [XmlElement("latestRepositoryId")]
        public string LatestRepostiroyId { get; set; }

        [XmlArray("artifactHits")]
        public ArtifactHit[] Hits { get; set; }

        public NexusArtifact()
        {
            this.ArtifactId = "";
            this.GroupId = "";
            this.LatestRelease = "";
            this.LatestRepostiroyId = "";
            this.Version = "";
            this.Hits = null;
        }
    }
}
