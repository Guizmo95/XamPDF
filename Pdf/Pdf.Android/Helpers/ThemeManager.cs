using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Pdf.Droid;
using Pdf.Interfaces;
using Plugin.CurrentActivity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.Dependency(typeof(ThemeManager))]
namespace Pdf.Droid
{

    public class ThemeManager : IThemeManager
    {
        private readonly Window window = MainActivity.MainActivityWindow;

        public void ChangeNavigationBarColor(Xamarin.Forms.Color color)
        {
            var androidColor = color.AddLuminosity(-0.1).ToAndroid();
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.Window.SetNavigationBarColor(androidColor);
        }

        public void SetPdfViewerStatusBarColor()
        {
            // The SetStatusBarcolor is new since API 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#fafafa"));
            }
            else
            {

                // Here you will just have to set your 
                // color in styles.xml file as shown below.
            }
        }

        public void SetMenuStatusBarColor()
        {
            // The SetStatusBarcolor is new since API 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#c62828"));
            }
            else
            {

                // Here you will just have to set your 
                // color in styles.xml file as shown below.
            }
        }

        public void SetFullScreen()
        {
            window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);
        }
    }
}