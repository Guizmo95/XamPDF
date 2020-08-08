using System;
using System.Runtime.CompilerServices;
using Android.Content;
using Android.Graphics.Pdf;
using Android.OS;
using Android.Provider;
using Java.IO;
using Javax.Xml.Parsers;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace Pdf.Droid.Helpers
{
    public class IntentHelper
    {
        private readonly MainActivity mainActivity;

        public IntentHelper(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public void GetFilePathFromIntent()
        {
            if (Intent.ActionView.Equals(mainActivity.Intent.Action) && (mainActivity.Intent.Type?.Equals("application/pdf") ?? false))
            {
                var path = GetRealPathFromUri(Android.App.Application.Context, mainActivity.Intent.Data);

                //TODO -- Implement try catch
                App.LoadPdf(path);
            }
        }

        public static string GetRealPathFromUri(Context context, Android.Net.Uri uri)
        {

            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                    return GetExternalStorageDocument(uri);
                // DownloadsProvider
                else if (IsDownloadsDocument(uri))
                    return GetDownloadDirectory(context, uri);
                // MediaProvider
                else if (IsMediaDocument(uri))
                    return GetMediaDirectory(context, uri);
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return GetDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        private static string GetExternalStorageDocument(Uri uri)
        {
            var docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
            var split = docId.Split(':');
            var type = split[0];

            return "primary".Equals(type, StringComparison.OrdinalIgnoreCase)
                ? Android.OS.Environment.ExternalStorageDirectory + "/" + split[1]
                : null;
        }

        private static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        private static string GetDownloadDirectory(Context context, Uri uri)
        {
            var id = Android.Provider.DocumentsContract.GetDocumentId(uri);
            var contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"),
                Convert.ToInt64(id));

            return GetDataColumn(context, contentUri, null, null);
        }

        private static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        private static string GetMediaDirectory(Context context, Uri uri)
        {
            var docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
            var split = docId.Split(':');
            var type = split[0];

            Uri contentUri = type switch
            {
                "image" => Android.Provider.MediaStore.Images.Media.ExternalContentUri,
                "video" => Android.Provider.MediaStore.Video.Media.ExternalContentUri,
                "audio" => Android.Provider.MediaStore.Audio.Media.ExternalContentUri,
                _ => null
            };

            const string selection = "_id=?";
            var selectionArgs = new string[]
            {
                split[1]
            };

            return GetDataColumn(context, contentUri, selection, selectionArgs);
        }

        public static string GetDataColumn(Context context, Android.Net.Uri uri, String selection,
            string[] selectionArgs)
        {
            Android.Database.ICursor cursor = null;
            const string column = "_data";
            string[] projection = {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                    null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    var column_index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(column_index);
                }
            }
            finally
            {
                cursor?.Close();
            }

            return null;
        }
    }
}