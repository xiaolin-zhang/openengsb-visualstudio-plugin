using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    class UIArtifactService
    {
        public static IList<UIArtifact> LoadUIArtifacts(Repository repo, Filter filter)
        {
            RepositoryService service = new RepositoryService(repo, filter);

            IList<Artifact> model_artifacts = service.LoadArtifacts();

            return LoadUIArtifacts(model_artifacts);
        }
        /// <summary>
        /// Mirroring the artifacts tree to a separate tree for ui reprentation.
        /// </summary>
        /// <returns></returns>
        public static IList<UIArtifact> LoadUIArtifacts(IList<Artifact> model_artifacts)
        {
            IList<UIArtifact> artifacts = new List<UIArtifact>();

            foreach(Artifact a  in model_artifacts)
            {
                UIArtifact ua = new UIArtifact(a);

                foreach (ItemVersion v in a.Versions)
                {
                    UIItemVersion uv = new UIItemVersion(v);

                    foreach (Item i in v.Items)
                    {
                        UIItem ui = new UIItem(i);
                        uv.Items.Add(ui);
                    }

                    ua.Versions.Add(uv);
                }

                artifacts.Add(ua);
            }

            return artifacts;
        }
    }
}
