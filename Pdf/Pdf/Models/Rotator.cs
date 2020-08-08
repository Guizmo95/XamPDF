using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Models
{
    public class Rotator
    {
        public Rotator(View itemTemplate)
        {
            ItemTemplate = itemTemplate;
        }

        public View ItemTemplate { get; set; }
    }
}
