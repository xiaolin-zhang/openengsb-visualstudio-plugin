using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Testing;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service
{
    public class RepositoryService
    {
        public Repository Repo { get; set; }
        private IRepositoryClient _client;

        public RepositoryService(Repository repository, Filter filter)
        {
            if (repository.RepoType == Repository.Type.Maven)
                _client = new MavenCentralClient(repository.Location, repository.GroupIds);
            else if (repository.RepoType == Repository.Type.Artifactory)
                _client = new ArtifactoryClient(repository.Location, repository.User, repository.Password, repository.GroupIds);
            else if (repository.RepoType == Repository.Type.Local)
                _client = new LocalRepositoryClient(repository.Location);
            else if (repository.RepoType == Repository.Type.Nexus)
                _client = new NexusClient(repository.Location, repository.User, repository.Password, repository.GroupIds);
            else
                throw new ApplicationException("Unknown repository client type!");

            if (filter != null)
                _client.ItemFilter = filter;

            Repo = repository;
        }

        public IList<Artifact> LoadArtifacts()
        {
            return _client.GetArtifacts();
        }

        private IList<Artifact> GenerateDefaultArtifacts()
        {
            DataGenerator gen = new DataGenerator();
            IList<Artifact> artifacts = new List<Artifact>();

            for (int i = 0; i < 2; i++)
            {
                Artifact a = gen.NextArtifact();
                for (int j = 0; j < 2; j++)
                {
                    ItemVersion v = gen.NextVersion(a);
                    for (int k = 0; k < 2; k++)
                    {
                        Item it = gen.NextItem(v);
                        v.Items.Add(it);
                    }
                    a.Versions.Add(v);
                }
                artifacts.Add(a);
            }

            return artifacts;
        }
    }
}
