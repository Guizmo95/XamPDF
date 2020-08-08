
using Pdf.Data;
using Pdf.Views;
using Xamarin.Forms;

namespace Pdf
{
    public partial class App : Application
    {
        private static FavoriteFilesDatabase database;
        private static ShellMainMenu mainMenu;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjk4MzY2QDMxMzgyZTMyMmUzMGY2UjRiblJ6SENGTERFSFRYOG5ZU2ptbmp5WUtWQmZScklQVHVZQ2pnRW89");

            InitializeComponent();

            mainMenu = new ShellMainMenu();

            MainPage = mainMenu;
        }

        public static FavoriteFilesDatabase Database => database ?? (database = new FavoriteFilesDatabase());

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static void LoadPdf(string url)
        {
            mainMenu.Navigation.PushAsync(new PdfViewer(url, Enumerations.LoadingMode.ByIntent));
        }
    }
}
