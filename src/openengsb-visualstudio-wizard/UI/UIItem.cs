using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.OpenEngSb.VisualStudio.Plugins.Wizards.Common;

namespace Org.OpenEngSb.VisualStudio.Plugins.Wizards.UI
{
    class UIItem
    {
        public Item ItemModel { get; set; }
        public bool IsChecked { get; set; }

        public UIItem(Item i)
        {
            ItemModel = i;
        }
    }
}
