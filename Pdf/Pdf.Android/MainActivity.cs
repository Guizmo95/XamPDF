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

namespace Pdf.Droid
{
    [Activity(Label = "Xam's Pdf", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Base", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "application/pdf", DataPathPattern = ".*\\\\topo")]
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
            Syncfusion.XForms.Android.PopupLayout.SfPopupLayoutRenderer.Init();

            LoadApplication(new App());

            var action = Intent.Action;
            var type = Intent.Type;

            if (Android.Content.Intent.ActionView.Equals(action) && (type?.Equals("application/pdf") ?? false))
            {
                var path = GetRealPathFromURI(Android.App.Application.Context, Intent.Data);

                App.LoadPDF(path);
            }

            static string GetRealPathFromURI(Context context, Android.Net.Uri uri)
            {

                bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

                // DocumentProvider
                if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
                {
                    // ExternalStorageProvider
                    if (isExternalStorageDocument(uri))
                    {
                        string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                        string[] split = docId.Split(':');
                        string type = split[0];

                        if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                        }

                        // TODO handle non-primary volumes
                    }
                    // DownloadsProvider
                    else if (isDownloadsDocument(uri))
                    {

                        string id = Android.Provider.DocumentsContract.GetDocumentId(uri);
                        Android.Net.Uri contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

                        return getDataColumn(context, contentUri, null, null);
                    }
                    // MediaProvider
                    else if (isMediaDocument(uri))
                    {
                        string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                        string[] split = docId.Split(':');
                        string type = split[0];

                        Android.Net.Uri contentUri = null;
                        if ("image".Equals(type))
                        {
                            contentUri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                        }
                        else if ("video".Equals(type))
                        {
                            contentUri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
                        }
                        else if ("audio".Equals(type))
                        {
                            contentUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                        }

                        string selection = "_id=?";
                        string[] selectionArgs = new string[] {
                    split[1]
            };

                        return getDataColumn(context, contentUri, selection, selectionArgs);
                    }
                }
                // MediaStore (and general)
                else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return getDataColumn(context, uri, null, null);
                }
                // File
                else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    return uri.Path;
                }

                return null;
            }

            static String getDataColumn(Context context, Android.Net.Uri uri, String selection,
                    String[] selectionArgs)
            {

                Android.Database.ICursor cursor = null;
                string column = "_data";
                string[] projection = {
                column
            };

                try
                {
                    cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                            null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        int column_index = cursor.GetColumnIndexOrThrow(column);
                        return cursor.GetString(column_index);
                    }
                }
                finally
                {
                    if (cursor != null)
                        cursor.Close();
                }
                return null;
            }

            static bool isExternalStorageDocument(Android.Net.Uri uri)
            {
                return "com.android.externalstorage.documents".Equals(uri.Authority);
            }

            /**
             * @param uri The Uri to check.
             * @return Whether the Uri authority is DownloadsProvider.
             */
            static bool isDownloadsDocument(Android.Net.Uri uri)
            {
                return "com.android.providers.downloads.documents".Equals(uri.Authority);
            }

            /**
             * @param uri The Uri to check.
             * @return Whether the Uri authority is MediaProvider.
             */
            static bool isMediaDocument(Android.Net.Uri uri)
            {
                return "com.android.providers.media.documents".Equals(uri.Authority);
            }
        }
    }
}
 