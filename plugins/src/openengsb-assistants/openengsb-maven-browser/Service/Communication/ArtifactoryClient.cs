using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;
using System.Net;
using System.Web.Script.Serialization;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication.JSON;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication
{
    public class ArtifactoryClient: IRepositoryClient
    {
        private WebClient _client;
        private string _baseUrl;
        private JavaScriptSerializer _serializer;
        private string _user;
        private string _password;
        private string[] _groupIds;

        public Filter ItemFilter { get; set; }

        public ArtifactoryClient(string baseUrl, string user, string password, string[] groupIds)
        {
            _client = new WebClient();
            _client.Credentials = new NetworkCredential(user, password);
            _user = user;
            _password = password;
            _baseUrl = baseUrl + "/api/search/artifact?";
            _serializer = new JavaScriptSerializer();
            _groupIds = groupIds;
            ItemFilter = (string s) => s.EndsWith("DomainEvents.wsdl") || s.EndsWith("Domain.wsdl");
        }

        private string getQuickSearchUrl(string groupId)
        {
            return _baseUrl + "name=" + groupId;
        }

        private string getArtifactsUrl(string groupId)
        {
            return _baseUrl + getArtifactsQuery(groupId);
        }

        private string getVersionsUrl(string groupId, string artifactId)
        {
            return _baseUrl + getVersionsQuery(groupId, artifactId);
        }

        private string getArtifactsQuery(string groupId)
        {
            return "g=" + groupId;
        }

        private string getVersionsQuery(string groupId, string artifactId)
        {
            return getArtifactsQuery(groupId) + "&a=" + artifactId;
        }

        public IList<Artifact> GetArtifacts()
        {
            List<Artifact> artifacts = new List<Artifact>();
            foreach (string groupId in _groupIds)
            {
                IList<Artifact> a = GetArtifacts(groupId);
                artifacts.AddRange(a);
            }
            return artifacts;
        }

        public IList<Artifact> GetArtifacts(string groupId)
        {
            string url = getQuickSearchUrl(groupId);
            string artifactsString = _client.DownloadString(url);

            IList<Artifact> artifacts = new List<Artifact>();

            ArtifactoryResponse response = _serializer.Deserialize<ArtifactoryResponse>(artifactsString);
            
            foreach (ArtifactoryResponse.Entry entry in response.results)
            {
                if(!ItemFilter(entry.uri))
                    continue;
                entry.uri = entry.uri.Replace("/api/storage", "");
                string name = entry.uri.Split('/').Last();
                Common.CreateArtifactsFromFileName(artifacts, entry.uri, name, _user, _password);
            }
            return artifacts;
        }

        public IList<ItemVersion> GetVersions()
        {
            throw new NotImplementedException();
        }

        public IList<Item> GetItems()
        {
            throw new NotImplementedException();
        }
    }
}
