using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Syncfusion.ListView.XForms;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private readonly SfPopupLayout deleteFilePopup;
        private Command<int> favoritesImageCommand;
        private Command<int> deleteImageCommand;
        private static ObservableCollection<FileModel> documents;
        internal SfListView sfListView;

        private string filePathToDelete;
        private FileModel itemToRemove;

        private readonly IPdfPicker pdfPickerAndroid;

        public static ObservableCollection<FileModel> Documents
        {
            get { return documents; }
            set { documents = value; }
        }

        public FileModel ItemToRemove
        {
            get { return itemToRemove; }
            set { SetProperty(ref itemToRemove, value); }
        }

        public Command<int> FavoritesImageCommand
        {
            get { return favoritesImageCommand; }
            protected set { SetProperty(ref favoritesImageCommand, value); }
        }

        public Command<int> DeleteImageCommand
        {
            get { return deleteImageCommand; }
            protected set { SetProperty(ref deleteImageCommand, value); }
        }

        public DocumentViewModel()
        {
            pdfPickerAndroid = DependencyService.Get<IPdfPicker>();

            Documents = new ObservableCollection<FileModel>();
            FavoritesDocumentViewModel.FavoritesDocuments = new ObservableCollection<FileModel>(App.Database.GetItemsAsync().Result);

            int i = 0;
            pdfPickerAndroid.GetPdfFilesInDocuments().ForEach(delegate (FileInfo fileInfo)
            {
                var fileModel = new FileModel()
                {
                    ItemIndexInDocumentList = i,
                    FileName = fileInfo.Name,
                    CreationTime = fileInfo.CreationTime.Date,
                    FileLenght = fileInfo.Length,
                    FilePath = fileInfo.FullName,
                };

                if ((FavoritesDocumentViewModel.FavoritesDocuments.Any(x => x.FilePath == fileInfo.FullName) == true))
                {
                    fileModel.IsFavorite = true;
                }
                else
                {
                    fileModel.IsFavorite = false;
                }

                fileModel.GetHumanReadableFileSize();

                Documents.Add(fileModel);

                i++;
            });

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
            favoritesImageCommand = new Command<int>((int itemIndex) => SetFavorites(itemIndex));
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
                    Documents.Remove(itemToRemove);

                    Documents.ToList().ForEach(delegate (FileModel fileModel)
                    {
                        if (fileModel.ItemIndexInDocumentList > itemToRemove.ItemIndexInDocumentList)
                        {
                            fileModel.ItemIndexInDocumentList = fileModel.ItemIndexInDocumentList - 1;
                        }
                    });

                    if (FavoritesDocumentViewModel.FavoritesDocuments.Contains(itemToRemove) == true)
                    {
                        FavoritesDocumentViewModel.FavoritesDocuments.Remove(ItemToRemove);

                        FavoritesDocumentViewModel.FavoritesDocuments.ToList().ForEach(delegate (FileModel fileModel)
                        {
                            if (fileModel.ItemIndexInFavoriteDocumentList > itemToRemove.ItemIndexInFavoriteDocumentList)
                            {
                                fileModel.ItemIndexInFavoriteDocumentList = fileModel.ItemIndexInFavoriteDocumentList - 1;
                            }
                        });

                    }
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

        public void SetFavorites(int itemIndex)
        {
            if (itemIndex >= 0)
            {
                var item = Documents[itemIndex];
                if (item.IsFavorite == true)
                {
                    Documents[Documents.IndexOf(item)].IsFavorite = false;
                    App.Database.DeleteItemAsync(item);
                    FavoritesDocumentViewModel.FavoritesDocuments.Remove(item);
                    DependencyService.Get<IToastMessage>().ShortAlert("File removed from favorites");
                }
                else
                {
                    App.Database.SaveItemAsync(item);
                    Documents[Documents.IndexOf(item)].IsFavorite = true;
                    item.IsFavorite = true;
                    //item.ItemIndex = FavoritesDocuments.Last().ItemIndex + 1;
                    if (FavoritesDocumentViewModel.FavoritesDocuments.Count != 0)
                        item.ItemIndexInFavoriteDocumentList += 1;
                    else
                        if (FavoritesDocumentViewModel.FavoritesDocuments.Count == 0)
                            item.ItemIndexInFavoriteDocumentList = 0;

                    FavoritesDocumentViewModel.FavoritesDocuments.Add(item);

                    DependencyService.Get<IToastMessage>().ShortAlert("File added to favorites");
                }
            }
            sfListView.ResetSwipe();
        }
    }
}

