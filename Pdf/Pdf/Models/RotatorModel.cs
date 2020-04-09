using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Models
{
    public class RotatorModel
    {
        public RotatorModel(View itemTemplate)
        {
            ItemTemplate = itemTemplate;
        }

        private View itemTemplate;
        public View ItemTemplate
        {
            get
            {
                return itemTemplate;
            }

            set
            {
                itemTemplate = value;
            }
        }
    }
}
