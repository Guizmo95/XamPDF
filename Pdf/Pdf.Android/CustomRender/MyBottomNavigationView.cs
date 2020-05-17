using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Pdf.Droid.CustomRender;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Pdf.Droid
{
    internal class MyBottomNavigationView : IShellBottomNavViewAppearanceTracker
    {
        private AndroidShell androidShell;

        public MyBottomNavigationView(AndroidShell androidShell)
        {
            this.androidShell = androidShell;
        }

        public void Dispose()
        {

        }

        public void ResetAppearance(BottomNavigationView bottomView)
        {

            
        }

        public void SetAppearance(BottomNavigationView bottomView, ShellAppearance appearance)
        {
            IMenu menu = bottomView.Menu;
            for (int i = 0; i < bottomView.Menu.Size(); i++)
            {
                IMenuItem menuItem = menu.GetItem(i);
                var title = menuItem.TitleFormatted;
                AssetManager assets = MainActivity.MyAssets;
                Typeface typeface = Typeface.CreateFromAsset(assets, "GothamBold.ttf");
                SpannableStringBuilder sb = new SpannableStringBuilder(title);

                sb.SetSpan(new CustomTypefaceSpan("", typeface), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                menuItem.SetTitle(sb);
            }

            int[][] states = new int[][]
            {
            //new int[] { Android.Resource.Attribute.StateEnabled}, // enabled
            new int[] {-Android.Resource.Attribute.StateSelected},
            new int[] {Android.Resource.Attribute.StateSelected},
            };

            int[] colors = new int[]
            {
            Xamarin.Forms.Color.FromHex("#bdbdbd").ToAndroid(),
            Xamarin.Forms.Color.FromHex("#ffffff").ToAndroid(),
            };

            ColorStateList myList = new ColorStateList(states, colors);

            bottomView.SetBackgroundColor(Android.Graphics.Color.ParseColor("#b71c1c"));

            bottomView.ItemTextColor = myList;
            bottomView.ItemIconTintList = myList;
        }
    }
}