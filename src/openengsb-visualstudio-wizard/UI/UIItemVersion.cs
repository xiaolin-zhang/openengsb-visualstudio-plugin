using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    class UIItemVersion
    {
        public ItemVersion ItemVersionModel { get; set; }
        public IList<UIItem> Items { get; set; }

        public UIItemVersion(ItemVersion version)
        {
            ItemVersionModel = version;
            Items = new List<UIItem>();
        }
    }
}
