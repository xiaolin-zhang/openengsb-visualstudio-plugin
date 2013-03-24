using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication.XML.Nexus
{
    [Serializable]
    [XmlType("artifactLink")]
    public class ArtifactLink
    {
        [XmlElement("classifier")]
        public string Classifier { get; set; }

        [XmlElement("extension")]
        public string Extension { get; set; }

        public ArtifactLink()
        {
            this.Classifier = "";
            this.Extension = "";
        }
    }
}
