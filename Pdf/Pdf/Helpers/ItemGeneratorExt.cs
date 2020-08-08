using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Helpers
{
    #region ItemGeneratorExt
    public class ItemGeneratorExt : ItemGenerator
    {
        public ItemGeneratorExt(SfListView listView) : base(listView)
        {

        }
        protected override ListViewItem OnCreateListViewItem(int itemIndex, ItemType type, object data = null)
        {
            return type == ItemType.Record 
                ? new ListViewItemExt(type) 
                    : base.OnCreateListViewItem(itemIndex, type, data);
        }
    }
    #endregion
}
