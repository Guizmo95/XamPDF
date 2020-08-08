using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DroidKeyboardHelper))]
namespace Pdf.Droid.Helpers
{
    public class DroidKeyboardHelper: IKeyboardHelper
    {
        public void HideKeyboard()
        {
            if (Android.App.Application.Context.GetSystemService(Context.InputMethodService) is InputMethodManager inputMethodManager && Android.App.Application.Context is Activity activity)
            {
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}