using Pdf.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = /*new NavigationPage(new MainMenu());*/
                new ShellMenu();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
