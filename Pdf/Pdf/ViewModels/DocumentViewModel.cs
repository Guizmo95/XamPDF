using Pdf.Interfaces;
using Pdf.Models;
using Syncfusion.ListView.XForms;
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
        private SfPopupLayout popupLayout;
        private Command favoritesImageCommand;
        private Command deleteImageCommand;
        private Command infoDocumentCommand;
        private ObservableCollection<FileModel> documents;
        private string filePathToDelete;
        private int itemIndex;




        private readonly IPdfPickerAndroid pdfPickerAndroid;
        internal SfListView sfListView;

        public ObservableCollection<FileModel> Documents 
        {
            get { return documents; }
            set { SetProperty(ref documents, value); }
        }
        public int ItemIndex
        {
            get { return itemIndex; }
            set { SetProperty(ref itemIndex, value); }
        }

        public Command FavoritesImageCommand
        {
            get { return favoritesImageCommand; }
            protected set { SetProperty(ref favoritesImageCommand, value); }
        }

        public Command DeleteImageCommand
        {
            get { return deleteImageCommand; }
            protected set { SetProperty(ref deleteImageCommand, value); }
        }

        public Command InfoDocumentCommand
        {
            get { return infoDocumentCommand; }
            protected set { SetProperty(ref infoDocumentCommand, value);}
        }

        public DocumentViewModel()
        {
            pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            Documents = new ObservableCollection<FileModel>();
            var favoriteFiles = App.Database.GetItemsAsync().Result;

            int i = 0;
            pdfPickerAndroid.GetPdfFilesInDocuments().ForEach(delegate (FileInfo fileInfo)
            {
                var fileModel = new FileModel()
                {
                    ItemIndex = i,
                    FileName = fileInfo.Name,
                    CreationTime = fileInfo.CreationTime.Date,
                    FileLenght = fileInfo.Length,
                    FilePath = fileInfo.FullName,
                };

                if ((favoriteFiles.Any(x => x.FileName == fileInfo.Name) == true)){
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

            popupLayout = new SfPopupLayout();
            popupLayout.PopupView.HeightRequest = 115;
            popupLayout.PopupView.WidthRequest = 310;
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = true;
            popupLayout.PopupView.ShowCloseButton = true;
            popupLayout.PopupView.AnimationDuration = 170;

            DataTemplate templateView = new DataTemplate(() =>
            {
                Label popupContent = new Label();
                popupContent.VerticalTextAlignment = TextAlignment.Center;
                popupContent.Padding = new Thickness(20);
                popupContent.LineBreakMode = LineBreakMode.MiddleTruncation;
                popupContent.Text = string.Format("{0} will be deleted !", Documents[ItemIndex].FileName);
                popupContent.TextColor = Color.Black;
                popupContent.FontSize = 13.5;
                popupContent.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                return popupContent;
            });

            DataTemplate footerTemplateView = new DataTemplate(() =>
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
            popupLayout.PopupView.ContentTemplate = templateView;

            // Adding FooterTemplate of the SfPopupLayout
            popupLayout.PopupView.FooterTemplate = footerTemplateView;

            deleteImageCommand = new Command(Delete);
            infoDocumentCommand = new Command(GetInfoDocument);
            favoritesImageCommand = new Command(SetFavorites);
        }

        private void DeclinePopupButton_Clicked(object sender, EventArgs e)
        {
            popupLayout.IsOpen = false;
            sfListView.ResetSwipe();
        }

        private void AcceptPopupButton_Clicked(object sender, System.EventArgs e)
        {
            if(String.IsNullOrEmpty(filePathToDelete) != true)
            {
                File.Delete(filePathToDelete);

                if (ItemIndex >= 0)
                    Documents.RemoveAt(ItemIndex);

                Documents.ToList().ForEach(c => c.Id = c.Id - 1);

                popupLayout.IsOpen = false;
            }

            sfListView.ResetSwipe();
        }

        private void Delete()
        {
            popupLayout.IsOpen = true;

            filePathToDelete = Documents[ItemIndex].FilePath;
        }

        private void GetInfoDocument()
        {
            IList<DocumentInfoListViewModel> documentInfoListViewModels = new List<DocumentInfoListViewModel>();

            //TODO -- GEt info doc method
           
        }

        private void SetFavorites()
        {
            if (ItemIndex >= 0)
            {
                var item = Documents[ItemIndex];
                if(item.IsFavorite == true)
                {
                    item.IsFavorite = false;
                    App.Database.DeleteItemAsync(item);

                }
                else
                {
                    item.IsFavorite = true;
                    App.Database.SaveItemAsync(item);
                }
            }
            sfListView.ResetSwipe();
        }




    }

    }
