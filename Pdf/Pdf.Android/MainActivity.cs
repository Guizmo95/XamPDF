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
using Android.Content;
using System.IO;
using Pdf.Droid.Helpers;
using Plugin.Permissions;

namespace Pdf.Droid
{
    [Activity(Label = "Xam's Pdf", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Base", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionOpenDocument }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "application/pdf", DataPathPattern = ".*\\\\topo")]
    public class MainActivity : FormsAppCompatActivity
    {
        public MainActivity()
        {
            IntentHelper = new IntentHelper(this);
        }

        public static AssetManager MyAssets { get; set; }

        public static Window MainActivityWindow { get; set; }

        public IntentHelper IntentHelper { get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            MyAssets = this.Assets;
            MainActivityWindow = this.Window;

            base.OnCreate(savedInstanceState);

            UserDialogs.Init(this);

            Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, 0);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, 0);

            LoadApplication(new App());

            //Open file from other apps
            IntentHelper.GetFilePathFromIntent();
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

}
 