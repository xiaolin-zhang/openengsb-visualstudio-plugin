using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Service.Communication.JSON
{
    public class ArtifactoryResponse
    {
        public Entry[] results;

        public class Entry
        {
            public string uri;
        }
    }
}
