using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDownloadManager))]
namespace Pdf.Droid.Helpers
{
    public class AndroidDownloadManager : IAndroidDownloadManager
    {
        public void Download(string uri, string fileName)
        {
            var manager = DownloadManager.FromContext(Android.App.Application.Context);
            try
            {
                var source = Android.Net.Uri.Parse(uri);
                
               DownloadManager.Request request = new DownloadManager.Request(source);

                //request.AddRequestHeader("attachment", "application/pdf");

                //Setting title of request
                request.SetTitle(fileName);

                //Setting description of request
                request.SetDescription("Your file is downloading");

                //set notification when download completed
                request.SetNotificationVisibility(Android.App.DownloadVisibility.VisibleNotifyCompleted);

                //Set the local destination for the downloaded file to a path within the application's external files directory
                request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, fileName);

                request.AllowScanningByMediaScanner();

                //Enqueue download and save the referenceId
                manager.Enqueue(request);
            }
            catch (IllegalArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}