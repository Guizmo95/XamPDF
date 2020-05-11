namespace Pdf.Interfaces
{
    public interface IThemeManager
    {
        void ChangeNavigationBarColor();
        Android.Graphics.Point getNavigationBarSize();
        Android.Graphics.Point getAppUsableScreenSize();
        Android.Graphics.Point getRealScreenSize();
    }
}