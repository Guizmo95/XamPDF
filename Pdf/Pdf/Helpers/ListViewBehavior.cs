using Java.Lang;
using Pdf.Views;
using Syncfusion.XForms.EffectsView;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class ListViewBehavior : Behavior<SfEffectsView>
    {
        protected override void OnAttachedTo(SfEffectsView bindable)
        {
            bindable.SelectionChanged += Bindable_SelectionChanged;
            bindable.AnimationCompleted += AnimationCompleted;
            base.OnAttachedTo(bindable);
        }

        private void Bindable_SelectionChanged(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var effectsView = sender as SfEffectsView;
                effectsView.ApplyEffects(SfEffects.Ripple);
            });
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            MessagingCenter.Send<ListViewBehavior>(this, "PushAsyncPage");
        }
        protected override void OnDetachingFrom(SfEffectsView bindable)
        {
            bindable.SelectionChanged -= Bindable_SelectionChanged;
            bindable.AnimationCompleted -= AnimationCompleted;
            base.OnDetachingFrom(bindable);
        }
    }
}
