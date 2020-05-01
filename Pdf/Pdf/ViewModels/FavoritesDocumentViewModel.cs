using Pdf.controls;
using Pdf.Interfaces;
using Pdf.Models;
using Syncfusion.ListView.XForms;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class FavoritesDocumentViewModel:BaseViewModel
    {
        private readonly SfPopupLayout deleteFilePopup;
        private Command<int> deleteImageCommand;
        private Command<int> deleteFromFavoritesCommand;

        private static ObservableCollection<FileModel> favoritesDocuments;
        internal SfListView sfListView;

        private string filePathToDelete;
        private FileModel itemToRemove;

        public static ObservableCollection<FileModel> FavoritesDocuments
        {
            get { return favoritesDocuments; }
            set { favoritesDocuments = value; }
        }
        public Command<int> DeleteFromFavoritesCommand
        {
            get { return deleteFromFavoritesCommand; }
            protected set { SetProperty(ref deleteFromFavoritesCommand, value); }
        }

        public FileModel ItemToRemove
        {
            get { return itemToRemove; }
            set { SetProperty(ref itemToRemove, value); }
        }

        public Command<int> DeleteImageCommand
        {
            get { return deleteImageCommand; }
            protected set { SetProperty(ref deleteImageCommand, value); }
        }

        public FavoritesDocumentViewModel()
        {
            deleteFilePopup = new SfPopupLayout();
            deleteFilePopup.PopupView.HeightRequest = 115;
            deleteFilePopup.PopupView.WidthRequest = 310;
            deleteFilePopup.PopupView.ShowHeader = false;
            deleteFilePopup.PopupView.ShowFooter = true;
            deleteFilePopup.PopupView.ShowCloseButton = true;
            deleteFilePopup.PopupView.AnimationDuration = 170;

            DataTemplate templateViewDeletePopup = new DataTemplate(() =>
            {
                Label popupContent = new Label();
                popupContent.VerticalTextAlignment = TextAlignment.Center;
                popupContent.Padding = new Thickness(20);
                popupContent.LineBreakMode = LineBreakMode.MiddleTruncation;
                popupContent.Text = string.Format("{0} will be deleted !", itemToRemove.FileName);
                popupContent.TextColor = Color.Black;
                popupContent.FontSize = 13.5;
                popupContent.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                return popupContent;
            });

            DataTemplate footerTemplateViewDeletePopup = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Horizontal;
                stackLayout.Spacing = 0;

                Button acceptButton = new Button();
                acceptButton.Text = "Accept";
                acceptButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                acceptButton.FontSize = 14;
                acceptButton.TextColor = Color.Black;
                acceptButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                acceptButton.BackgroundColor = Color.Transparent;
                acceptButton.VerticalOptions = LayoutOptions.Center;
                acceptButton.WidthRequest = 86;
                acceptButton.Clicked += AcceptPopupButton_Clicked;
                Button declineButton = new Button();
                declineButton.Text = "Decline";
                declineButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                declineButton.FontSize = 14;
                declineButton.TextColor = Color.Black;
                declineButton.HorizontalOptions = LayoutOptions.End;
                declineButton.BackgroundColor = Color.Transparent;
                declineButton.VerticalOptions = LayoutOptions.Center;
                declineButton.WidthRequest = 89;
                declineButton.Clicked += DeclinePopupButton_Clicked;

                stackLayout.Children.Add(acceptButton);
                stackLayout.Children.Add(declineButton);

                return stackLayout;
            });

            // Adding ContentTemplate of the SfPopupLayout
            deleteFilePopup.PopupView.ContentTemplate = templateViewDeletePopup;

            // Adding FooterTemplate of the SfPopupLayout
            deleteFilePopup.PopupView.FooterTemplate = footerTemplateViewDeletePopup;

            deleteImageCommand = new Command<int>((int itemIndex) => Delete(itemIndex));
            FavoritesDocuments = new ObservableCollection<FileModel>(App.Database.GetItemsAsync().Result);
            deleteFromFavoritesCommand = new Command<int>((int itemIndex) => RemoveFromFavorites(itemIndex));
        }

        public void DeclinePopupButton_Clicked(object sender, EventArgs e)
        {
            deleteFilePopup.IsOpen = false;
            sfListView.ResetSwipe();
        }

        public void AcceptPopupButton_Clicked(object sender, System.EventArgs e)
        {
            if (String.IsNullOrEmpty(filePathToDelete) != true)
            {
                File.Delete(filePathToDelete);

                if (itemToRemove != null)
                {
                    FavoritesDocuments.Remove(itemToRemove);
                    DocumentViewModel.Documents.Remove(itemToRemove);

                    FavoritesDocuments.ToList().ForEach(delegate (FileModel fileModel)
                    {
                        if (fileModel.ItemIndexInFavoriteDocumentList > itemToRemove.ItemIndexInFavoriteDocumentList)
                        {
                            fileModel.ItemIndexInFavoriteDocumentList = fileModel.ItemIndexInFavoriteDocumentList - 1;
                        }
                    });

                    DocumentViewModel.Documents.ToList().ForEach(delegate (FileModel fileModel)
                    {
                        if (fileModel.ItemIndexInDocumentList > itemToRemove.ItemIndexInDocumentList)
                        {
                            fileModel.ItemIndexInDocumentList = fileModel.ItemIndexInDocumentList - 1;
                        }
                    });
                }

                deleteFilePopup.IsOpen = false;
            }
            sfListView.ResetSwipe();
        }

        public void Delete(int itemIndex)
        {
            deleteFilePopup.IsOpen = true;

            filePathToDelete = itemToRemove.FilePath;
        }

        public void RemoveFromFavorites(int itemIndex)
        {
            if (itemIndex >= 0)
            {
                var item = FavoritesDocuments[itemIndex];
                if (item.IsFavorite == true)
                {
                    DocumentViewModel.Documents.First(c => c.FilePath == item.FilePath).IsFavorite = false;
                    FavoritesDocuments.Remove(item);
                    App.Database.DeleteItemAsync(item);
                }
            }
            sfListView.ResetSwipe();
            DependencyService.Get<IToastMessage>().ShortAlert("File removed from favorites");
        }
    }
}
