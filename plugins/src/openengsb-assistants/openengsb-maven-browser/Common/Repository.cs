using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common
{
    public class Repository
    {
        public enum Type { Local, Maven, Artifactory, Nexus }

        public string Location { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public Type RepoType { get; set; }
        public string[] GroupIds { get; set; }

        public Repository(): this("", "", "", Type.Local, null)
        {
        }

        public Repository(string location, string user, string password, Type type, string[] groupIds)
        {
            Location = location;
            User = user;
            Password = password;
            RepoType = type;
            GroupIds = groupIds;
            if (GroupIds == null)
            {
                GroupIds = new string[1];
                GroupIds[0] = "org.openengsb.domain";
            }
        }
    }
}
