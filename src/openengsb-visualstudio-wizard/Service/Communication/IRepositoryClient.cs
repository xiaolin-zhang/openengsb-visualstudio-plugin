using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service
{
    public delegate bool Filter(String s);
    public interface IRepositoryClient
    {
        IList<Artifact> GetArtifacts();
        Filter ItemFilter { get; set; }
    }
}
