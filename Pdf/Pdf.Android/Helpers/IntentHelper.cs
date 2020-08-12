using System;
using System.IO;
using Android.Content;
using Android.OS;
using Android.Provider;
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
                var filePath = GetRealPathFromUri(Android.App.Application.Context, mainActivity.Intent.Data);

                //TODO -- Handle when file is not downloaded

                var stream = mainActivity.ContentResolver.OpenInputStream(mainActivity.Intent.Data);

                if (string.IsNullOrEmpty(filePath))
                {
                    
                }

                //TODO -- Implement try catch
                App.LoadPdf(filePath);
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

        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection,
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

                //cursor = context.ContentResolver.Query(uri, new[] {
                //    MediaStore.MediaColumns.DisplayName,
                //}, null, null, null);

                if (cursor != null && cursor.MoveToFirst())
                {
                    var columnIndex = cursor.GetColumnIndexOrThrow(column);
                    var filePath = cursor.GetString(columnIndex);

                    if (string.IsNullOrEmpty(filePath))
                    {
                        cursor = context.ContentResolver.Query(uri, new[] {
                            MediaStore.MediaColumns.DisplayName,
                        }, null, null, null);

                        if (cursor != null && cursor.MoveToFirst())
                        {
                            columnIndex = cursor.GetColumnIndex(MediaStore.MediaColumns.DisplayName);

                            var fileName = cursor.GetString(columnIndex);

                            string downloaDirectory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                Android.OS.Environment.DirectoryDownloads);
                            
                            filePath = Path.Combine(downloaDirectory, fileName);
                        }

                    }

                    return filePath;
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