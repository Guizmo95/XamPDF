namespace Pdf.Interfaces
{
    public interface IThemeManager
    {
        void ChangeNavigationBarColor(Xamarin.Forms.Color color);
        void SetMenuStatusBarColor();
        void SetPdfViewerStatusBarColor();
        void SetFullScreen();
    }
}