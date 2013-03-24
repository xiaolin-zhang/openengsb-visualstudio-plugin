using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication.XML.Nexus;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication
{
    public class NexusClient : IRepositoryClient
    {
        private const string _ARTIFACT_EXTENSION = "wsdl";

        private string _baseUrl;
        private string _user;
        private string _password;
        private WebClient _client;
        private XmlSerializer _serializer;
        private string[] _groupdIds;

        public Filter ItemFilter { get; set; }

        public NexusClient(string url, string user, string password, string[] groupIds)
        {
            _baseUrl = url;
            _user = user;
            _password = password;
            _client = new WebClient();
            _client.Credentials = new NetworkCredential(_user, _password);
            _serializer = new XmlSerializer(typeof(NexusResponse));
            _groupdIds = groupIds;
            ItemFilter = (string s) => matchArtifact(s.Split(' ')[0], s.Split(' ')[1]);
        }

        private string getArtifactsUrl(string groupId)
        {
            return _baseUrl + "/service/local/lucene/search?q=" + groupId;
        }

        public IList<Artifact> GetArtifacts()
        {
            List<Artifact> artifacts = new List<Artifact>();
            foreach (string groupId in _groupdIds)
            {
                IList<Artifact> a = GetArtifacts(groupId);
                artifacts.AddRange(a);
            }
            return artifacts;
        }

        public IList<Artifact> GetArtifacts(string groupdId)
        {
            string rsp = _client.DownloadString(getArtifactsUrl(groupdId));
            StringReader reader = new StringReader(rsp);
            NexusResponse nexusRsp = (NexusResponse)_serializer.Deserialize(reader);

            IList<Artifact> artifacts = new List<Artifact>();

            foreach (NexusArtifact nart in nexusRsp.RspData)
            {
                foreach (ArtifactHit hit in nart.Hits)
                {
                    foreach (ArtifactLink link in hit.Links)
                    {
                        if(ItemFilter(link.Classifier+"." +link.Extension))
                        {
                            Artifact art = Common.GetOrCreateArtifact(artifacts, nart.ArtifactId);
                            ItemVersion ver = Common.GetOrCreateVersion(art, nart.Version);
                            string url = createNexusUrl(hit.RepoId, nart.GroupId, nart.ArtifactId, nart.Version, link.Classifier, link.Extension);
                            string fileName = createFileName(nart.ArtifactId, nart.Version, link.Classifier, link.Extension);
                            ver.Items.Add(new Item(fileName, url, ver));
                        }
                    }
                }
            }

            return artifacts;
        }

        private bool matchArtifact(string classifier, string extension)
        {
            if (!extension.ToLower().EndsWith(_ARTIFACT_EXTENSION))
                return false;

            return classifier.EndsWith("Domain") || classifier.EndsWith("DomainEvents");
        }

        private string createNexusUrl(string repository, string groupId, string artifactId, string version, string classifier, string extension)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_baseUrl);
            builder.Append("/content/repositories/");
            builder.Append(repository);
            builder.Append("/");
            builder.Append(groupId.Replace('.', '/'));
            builder.Append("/");
            builder.Append(artifactId);
            builder.Append("/");
            builder.Append(version);
            builder.Append("/");
            builder.Append(createFileName(artifactId, version, classifier, extension));
            return builder.ToString();
        }

        private string createFileName(string artifactId, string version, string classifier, string extension)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(artifactId);
            builder.Append("-");
            builder.Append(version);
            builder.Append("-");
            builder.Append(classifier);
            builder.Append(".");
            builder.Append(extension);
            return builder.ToString();
        }
    }
}
