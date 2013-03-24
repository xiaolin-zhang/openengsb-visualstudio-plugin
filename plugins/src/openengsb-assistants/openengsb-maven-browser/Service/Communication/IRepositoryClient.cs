using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Common;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Assistants.Service
{
    public delegate bool Filter(String s);
    public interface IRepositoryClient
    {
        IList<Artifact> GetArtifacts();
        Filter ItemFilter { get; set; }
    }
}
