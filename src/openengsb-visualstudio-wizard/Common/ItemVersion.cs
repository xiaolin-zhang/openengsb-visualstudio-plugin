﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common
{
    public class ItemVersion
    {
        public string Id { get; set; }
        public IList<Item> Items { get; set; }
        public Artifact ParentArtifact { get; set; }
        public ItemVersion(string id, Artifact artifact)
        {
            Id = id;
            Items = new List<Item>();
            ParentArtifact = artifact;
        }
    }
}
