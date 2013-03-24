using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service.Communication
{
    class Common
    {
        private const string GENERIC_GROUP = "openengsb";

        public static Artifact CreateArtifactsFromFileName(IList<Artifact> artifacts, string location, string name, string user, string password)
        {
            string[] subs = name.Split('-');
            if (subs.Length != 3)
                return null;

            Artifact a = GetOrCreateArtifact(artifacts, subs[0]);
            ItemVersion v = GetOrCreateVersion(a, subs[1]);
            Item i = new Item(name, location, v);
            i.User = user;
            i.Password = password;
            v.Items.Add(i);

            return a;
        }

        public static Artifact CreateArtifactsFromFileName(IList<Artifact> artifacts, string location, string name)
        {
            return CreateArtifactsFromFileName(artifacts, location, name, "", "");
        }

        public static ItemVersion GetOrCreateVersion(Artifact artifact, string version)
        {
            foreach (ItemVersion iv in artifact.Versions)
            {
                if (iv.Id == version)
                {
                    return iv;
                }
            }

            ItemVersion ver = new ItemVersion(version, artifact);
            artifact.Versions.Add(ver);
            return ver;
        }

        public static Artifact GetOrCreateArtifact(IList<Artifact> artifacts, string id)
        {
            foreach (Artifact a in artifacts)
            {
                if (a.Id == id)
                {
                    return a;
                }
            }

            Artifact art = new Artifact(GENERIC_GROUP, id);
            artifacts.Add(art);
            return art;
        }
    }
}
