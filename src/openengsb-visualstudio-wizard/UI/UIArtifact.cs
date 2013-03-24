using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    class UIArtifact
    {
        public Artifact ArtifactModel { get; set; }
        public IList<UIItemVersion> Versions { get; set; }

        public UIArtifact(Artifact artifact)
        {
            ArtifactModel = artifact;
            Versions = new List<UIItemVersion>();
        }
    }
}
