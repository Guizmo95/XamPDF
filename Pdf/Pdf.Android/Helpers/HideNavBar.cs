﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(HideNavBar))]
namespace Pdf.Droid.Helpers
{
    public class HideNavBar : IHideNavBar
    {
        Window window = MainActivity.MainActivityWindow;

        public void RemoveNavBar()
        {
            //window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.HideNavigation;

            int uiOptions = (int)window.DecorView.SystemUiVisibility;

            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

        }

        public void SetNavBar()
        {
            window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
        }

    }
}