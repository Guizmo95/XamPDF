using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.controls
{
    public class ExtendedButton : Xamarin.Forms.Button
    {
        public static BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create<ExtendedButton, Xamarin.Forms.TextAlignment>(x => x.HorizontalTextAlignment, Xamarin.Forms.TextAlignment.Center);
        public Xamarin.Forms.TextAlignment HorizontalTextAlignment
        {
            get
            {
                return (Xamarin.Forms.TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            }
            set
            {
                SetValue(HorizontalTextAlignmentProperty, value);
            }
        }
    }
}
