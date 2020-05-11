using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Helpers
{
    #region ItemGeneratorExt
    public class ItemGeneratorExt : ItemGenerator
    {
        public ItemGeneratorExt(SfListView listview) : base(listview)
        {

        }
        protected override ListViewItem OnCreateListViewItem(int itemIndex, ItemType type, object data = null)
        {
            if (type == ItemType.Record)
                return new ListViewItemExt(type);
            return base.OnCreateListViewItem(itemIndex, type, data);
        }
    }
    #endregion
}
