using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    #region ListViewItemExt
    public class ListViewItemExt : ListViewItem
    {
        public ListViewItemExt()
        {
        }
        public ListViewItemExt(ItemType type) : base(type)
        {

        }
        protected override void OnItemAppearing()
        {
            this.Opacity = 0;
            this.FadeTo(1, 400, Easing.SinInOut);
            base.OnItemAppearing();
        }
    }
    #endregion
}
