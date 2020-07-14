
using Pdf.Data;
using Pdf.ViewModels;
using Pdf.Views;
using System;
using System.ComponentModel;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    public partial class App : Application
    {
        public static UnityContainer Container { get; set; }
        static FavoriteFilesDatabase database;
        static ShellMainMenu mainMenu;

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjUxNzQ0QDMxMzgyZTMxMmUzMEtndVlMY3hrWXdxOTNraGZJWTJvcGdJazRjKzhjOVFTWlU5VDlmdnhhalk9");

            InitializeComponent();

            Container = new UnityContainer();

            mainMenu = new ShellMainMenu();

            MainPage = mainMenu;
        }

        public static FavoriteFilesDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new FavoriteFilesDatabase();
                }
                return database;
            }
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

        public static void LoadPDF(string url)
        {
            mainMenu.Navigation.PushAsync(new PdfViewer(url, Enumerations.LoadingMode.ByIntent));
        }
    }
}
