
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Pdf.Droid;
using Pdf.Droid.CustomRender;
using System.Windows.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;



[assembly: ExportRenderer(typeof(Xamarin.Forms.Frame), typeof(MyFrameRender))]
namespace Pdf.Droid.CustomRender
{
    public class MyFrameRender : FrameRenderer
    {

        public MyFrameRender(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && e.OldElement == null)
            {
                this.SetBackgroundResource(Resource.Drawable.FrameRenderValue);
                //GradientDrawable drawable = (GradientDrawable)this.Background;
                //drawable.SetColor(Android.Graphics.Color.ParseColor("#F0F0F0"));
            }
        }
    }
}