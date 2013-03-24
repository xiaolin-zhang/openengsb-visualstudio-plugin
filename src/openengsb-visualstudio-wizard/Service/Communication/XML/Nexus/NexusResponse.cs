using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication.XML.Nexus
{
    [Serializable]
    [XmlType("searchNGResponse")]
    public class NexusResponse
    {
        [XmlElement("totalCount")]
        public int TotalCount { get; set; }

        [XmlElement("from")]
        public string From { get; set; }

        [XmlElement("count")]
        public string Count { get; set; }

        [XmlElement("tooManyResults")]
        public bool TooManyResults { get; set; }

        [XmlElement("collapsed")]
        public bool Collapsed { get; set; }

        [XmlArray("repoDetails")]
        public RepoDetail[] Details { get; set; }

        [XmlArray("data")]
        public NexusArtifact[] RspData { get; set; }

        public NexusResponse()
        {
            this.Collapsed = false;
            this.Count = "";
            this.From = "";
            this.TooManyResults = false;
            this.TotalCount = -1;
            this.Details = null;
            this.RspData = null;
        }
    }
}
