using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pdf.controls;
using Pdf.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace Pdf.Droid
{
    public class ExtendedButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
    {

        public ExtendedButtonRenderer(Context context) :base(context)
        {

        }
        public new ExtendedButton Element
        {
            get
            {
                return (ExtendedButton)base.Element;
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                return;
            }

            SetTextAlignment();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ExtendedButton.HorizontalTextAlignmentProperty.PropertyName)
            {
                SetTextAlignment();
            }
        }

        public void SetTextAlignment()
        {
            Control.Gravity = Element.HorizontalTextAlignment.ToHorizontalGravityFlags();
        }
    }

    public static class AlignmentHelper
    {
        public static GravityFlags ToHorizontalGravityFlags(this Xamarin.Forms.TextAlignment alignment)
        {
            if (alignment == Xamarin.Forms.TextAlignment.Center)
                return GravityFlags.AxisSpecified;
            return alignment == Xamarin.Forms.TextAlignment.End ? GravityFlags.Right : GravityFlags.Left;
        }
    }
}