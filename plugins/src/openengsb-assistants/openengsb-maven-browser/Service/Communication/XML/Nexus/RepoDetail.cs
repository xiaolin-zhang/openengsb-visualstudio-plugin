using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication.XML.Nexus
{
    [Serializable]
    [XmlType("org.sonatype.nexus.rest.model.NexusNGRepositoryDetail")]
    public class RepoDetail
    {
        [XmlElement("repositoryId")]
        public string Id { get; set; }

        [XmlElement("repositoryName")]
        public string Name { get; set; }

        [XmlElement("repositoryContentClass")]
        public string ContentClass { get; set; }

        [XmlElement("repositoryKind")]
        public string Kind { get; set; }

        [XmlElement("repositoryPolicy")]
        public string Policy { get; set; }

        [XmlElement("repositoryUrl")]
        public string Url { get; set; }

        public RepoDetail()
        {
            this.ContentClass = "";
            this.Id = "";
            this.Kind = "";
            this.Name = "";
            this.Policy = "";
            this.Url = "";
        }
    }
}
