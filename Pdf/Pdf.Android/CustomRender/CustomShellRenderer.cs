using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Pdf.controls;
using Pdf.Droid;
using Pdf.Droid.CustomRender;
using Pdf.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ShellMainMenu), typeof(AndroidShell))]
namespace Pdf.Droid.CustomRender
{
    public class AndroidShell : ShellRenderer
    {
        public AndroidShell(Context context) : base(context)
        {
        }

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new MyBottomNavigationView(this);
        }
    }
}