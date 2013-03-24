using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication.JSON.MavenCentral
{
    public class ArtifactsResponse
    {
        public int numFound;
        public int start;
        public ArtifactsDoc[] docs;
    }
}
