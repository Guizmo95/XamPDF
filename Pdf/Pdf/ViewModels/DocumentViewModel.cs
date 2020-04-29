using Pdf.controls;
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
        private readonly SfPopupLayout getInfoFilePopup;
        private readonly PdfPropertyPopup pdfPropertyPopup;
        private Command favoritesImageCommand;
        private Command deleteImageCommand;
        private Command infoDocumentCommand;
        private ObservableCollection<FileModel> documents;
        internal SfListView sfListView;

        private string filePathToDelete;
        private int itemIndex;

        private readonly IPdfPickerAndroid pdfPickerAndroid;
        

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
                popupContent.Text = string.Format("{0} will be deleted !", Documents[ItemIndex].FileName);
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

            getInfoFilePopup = new SfPopupLayout();
            getInfoFilePopup.PopupView.IsFullScreen = true;
            getInfoFilePopup.PopupView.AnimationDuration = 200;
            getInfoFilePopup.PopupView.AnimationMode = AnimationMode.SlideOnBottom;
            getInfoFilePopup.PopupView.PopupStyle.CornerRadius = 0;
            getInfoFilePopup.PopupView.PopupStyle.BorderThickness = 2;
            getInfoFilePopup.PopupView.PopupStyle.BorderColor = Color.White;
            getInfoFilePopup.PopupView.ShowFooter = false;
            getInfoFilePopup.Closing += GetInfoFilePopup_Closing;
            getInfoFilePopup.PopupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#eeeeee");
            getInfoFilePopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#e0e0e0");

            pdfPropertyPopup = new PdfPropertyPopup();

            DataTemplate templateViewGetInfoPopup = new DataTemplate(() =>
            {
                return pdfPropertyPopup;
            });

            DataTemplate headerTemplateViewGetInfoPopup = new DataTemplate(() =>
            {
                Label title = new Label();
                title.Text = "Properties";
                title.FontSize = 18;
                title.FontFamily = "GothamBold.ttf#GothamBold";
                title.VerticalTextAlignment = TextAlignment.Center;
                title.HorizontalTextAlignment = TextAlignment.Center;
                title.TextColor = Color.FromHex("#4e4e4e");

                return title;
            });

            // Adding ContentTemplate of the SfPopupLayout
            getInfoFilePopup.PopupView.ContentTemplate = templateViewGetInfoPopup;

            // Adding HeaderTemplate of the SfPopupLayout
            getInfoFilePopup.PopupView.HeaderTemplate = headerTemplateViewGetInfoPopup;

            deleteImageCommand = new Command(Delete);
            infoDocumentCommand = new Command<int>((int itemIndex) => GetInfoDocument(itemIndex));
            favoritesImageCommand = new Command(SetFavorites);
        }

        private void GetInfoFilePopup_Closing(object sender, Syncfusion.XForms.Core.CancelEventArgs e)
        {
            sfListView.ResetSwipe();
        }

        private void DeclinePopupButton_Clicked(object sender, EventArgs e)
        {
            deleteFilePopup.IsOpen = false;
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

                deleteFilePopup.IsOpen = false;
            }

            sfListView.ResetSwipe();
        }

        private void Delete()
        {
            deleteFilePopup.IsOpen = true;

            filePathToDelete = Documents[ItemIndex].FilePath;
        }

        private void GetInfoDocument(int itemIndex)
        {
            using (Stream docStream = new FileStream(Documents[itemIndex].FilePath, FileMode.Open))
            {
                using (PdfLoadedDocument document = new PdfLoadedDocument(docStream))
                {
                    IList<DocumentInfoListViewModel> documentInfoListViewModels = new List<DocumentInfoListViewModel>();
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Filename", Documents[itemIndex].FileName));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Pages numbers", document.PageCount.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Author", document.DocumentInformation.Author));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creation date", document.DocumentInformation.CreationDate.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creator", document.DocumentInformation.Creator));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Keywords", document.DocumentInformation.Keywords));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Subject", document.DocumentInformation.Subject));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Title", document.DocumentInformation.Title));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Last modification", document.DocumentInformation.ModificationDate.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creation date", Documents[ItemIndex].CreationDate));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Producer", document.DocumentInformation.Producer));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("File size", Documents[ItemIndex].Size));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("File path", Documents[ItemIndex].FilePath));

                    pdfPropertyPopup.PdfPropertyListView.ItemsSource = documentInfoListViewModels;

                    getInfoFilePopup.IsOpen = true;
                }
            }
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
            DependencyService.Get<IToastMessage>().ShortAlert("File added to favorites");
        }
    }
}
