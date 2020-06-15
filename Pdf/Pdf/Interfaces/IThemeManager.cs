namespace Pdf.Interfaces
{
    public interface IThemeManager
    {
        void ChangeNavigationBarColor(Xamarin.Forms.Color color);
        void SetMenuStatusBarColor();
        Android.Graphics.Point getNavigationBarSize();
        Android.Graphics.Point getAppUsableScreenSize();
        Android.Graphics.Point getRealScreenSize();
        void SetPdfViewerStatusBarColor();
        void SetFullScreen();
    }
}