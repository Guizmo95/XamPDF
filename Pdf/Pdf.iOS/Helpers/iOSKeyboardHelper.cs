using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Pdf.Interfaces;
using Pdf.iOS.Helpers;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(iOSKeyboardHelper))]
namespace Pdf.iOS.Helpers
{
    public class iOSKeyboardHelper:IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}