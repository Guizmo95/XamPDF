using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pdf.Models;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.PopupLayout;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public abstract class BaseViewModel
    {
        protected Command<int> deleteImageCommand;
        protected SfPopupLayout deleteFilePopup;
        protected string filePathToDelete;
        
        internal SfListView sfListView;

        public FileModel ItemToRemove { get; set; }
        public Command<int> DeleteImageCommand
        {
            get => deleteImageCommand;
            protected set => SetProperty(ref deleteImageCommand, value);
        }

        protected BaseViewModel()
        {
            InitializeDeleteFilesPopup();

            DeleteImageCommand = new Command<int>((int itemIndex) => Delete(itemIndex));
        }

        private void InitializeDeleteFilesPopup()
        {
            deleteFilePopup = new SfPopupLayout
            {
                PopupView =
                {
                    HeightRequest = 115,
                    WidthRequest = 310,
                    ShowHeader = false,
                    ShowFooter = true,
                    ShowCloseButton = true,
                    AnimationDuration = 170
                }
            };

            var templateViewDeletePopup = new DataTemplate(() =>
            {
                var popupContent = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(20),
                    LineBreakMode = LineBreakMode.MiddleTruncation,
                    Text = $"{ItemToRemove.FileName} will be deleted !",
                    TextColor = Color.Black,
                    FontSize = 13.5,
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1"
                };
                return popupContent;
            });

            var footerTemplateViewDeletePopup = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout
                { Orientation = StackOrientation.Horizontal, Spacing = 0 };

                var acceptButton = new Button
                {
                    Text = "Accept",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    FontSize = 14,
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 86
                };
                acceptButton.Clicked += DeleteFileButton_Clicked;
                var declineButton = new Button
                {
                    Text = "Decline",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    FontSize = 14,
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.End,
                    BackgroundColor = Color.Transparent,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 89
                };
                declineButton.Clicked += DeclinePopupButton_Clicked;

                stackLayout.Children.Add(acceptButton);
                stackLayout.Children.Add(declineButton);

                return stackLayout;
            });

            // Adding ContentTemplate of the SfPopupLayout
            deleteFilePopup.PopupView.ContentTemplate = templateViewDeletePopup;

            // Adding FooterTemplate of the SfPopupLayout
            deleteFilePopup.PopupView.FooterTemplate = footerTemplateViewDeletePopup;
        }

        public void Delete(int itemIndex)
        {
            deleteFilePopup.IsOpen = true;

            filePathToDelete = ItemToRemove.FilePath;
        }

        public void DeclinePopupButton_Clicked(object sender, EventArgs e)
        {
            deleteFilePopup.IsOpen = false;
            sfListView.ResetSwipe();
        }

        protected abstract void DeleteFileButton_Clicked(object o, EventArgs args);
        protected abstract void DecreaseItemIndexForEachPdfFiles();

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName]string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
