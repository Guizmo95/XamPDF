using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace Pdf.Droid.Helpers
{
    public class ToastMessage : IToastMessage
    {
        public void LongAlert(string message)
        {
            Android.Widget.Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Android.Widget.Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}