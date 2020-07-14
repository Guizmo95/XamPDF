using Pdf.Models;
using Pdf.Views;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class SfEffectView : Behavior<SfEffectsView>
    {
        #region Fields 
        SfEffectsView EffectsView;

        #endregion

        protected override void OnAttachedTo(SfEffectsView bindable)
        {
            EffectsView = bindable as SfEffectsView;
            EffectsView.SelectionChanged += EffectsView_SelectionChanged;
            EffectsView.AnimationCompleted += EffectsView_AnimationCompleted;

            base.OnAttachedTo(bindable);
        }

        private async void EffectsView_AnimationCompleted(object sender, EventArgs e)
        {
            var args = ((sender as SfEffectsView).BindingContext as FileModel);
            await App.Current.MainPage.Navigation.PushAsync(new PdfViewer(args.FilePath, Enumerations.LoadingMode.ByDefault));
        }

        private void EffectsView_SelectionChanged(object sender, EventArgs e)
        {
            EffectsView.ApplyEffects();
        }
    }
}
