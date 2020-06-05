using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Pdf.Views;
using Android.Content.Res;

namespace Pdf.Droid
{
    [Activity(Label = "Xam's Pdf", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Base", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static AssetManager assets;
        static Window window;

        public static AssetManager MyAssets
        {
            get
            {
                return assets;
            }
            set
            {
                assets = value;
            }
        }

        public static Window MainActivityWindow
        {
            get
            {
                return window;
            }
            set
            {
                window = value;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            assets = this.Assets;
            window = this.Window;

            this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var stBarHeight = typeof(FormsAppCompatActivity).GetField("statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (stBarHeight == null)
                {
                    stBarHeight = typeof(FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                }
                stBarHeight?.SetValue(this, 0);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);



            base.OnCreate(savedInstanceState);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 0);
            }


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 0);
            }


            UserDialogs.Init(this);



        global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            Forms.SetFlags("IndicatorView_Experimental");
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            //Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }
}