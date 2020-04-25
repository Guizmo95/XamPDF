
using Pdf.Data;
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

        public App()
        {
            InitializeComponent();

            Container = new UnityContainer();


            MainPage = /*new NavigationPage(new MainMenu());*/
                new NavigationPage(new MainPage());
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
    }
}
