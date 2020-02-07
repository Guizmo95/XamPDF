using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pdf.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidWritter))]
namespace Pdf.Droid
{
    public class AndroidWritter:IAndroidWritter 
    {
        public string CreateFile(string filename, byte[] bytes)
        {
            if (!Directory.Exists(Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "YourAppName")))
                Directory.CreateDirectory(Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "YourAppName"));

            var path = Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "YourAppName", filename);

            File.WriteAllBytes(path, bytes);

            return path;
        }
    }
}