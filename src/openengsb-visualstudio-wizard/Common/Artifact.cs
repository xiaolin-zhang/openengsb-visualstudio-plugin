﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common
{
    public class Artifact
    {
        public string GroupId { get; set; }
        public string Id { get; set; }
        public IList<ItemVersion> Versions { get; set; }

        public Artifact(string groupid, string id)
        {
            GroupId = groupid;
            Id = id;
            Versions = new List<ItemVersion>();
        }
    }
}
