using Pdf.controls;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class SfBehaviorFavoritesPage: SfBehaviorBase
    {
        protected FavoritesDocumentViewModel viewModel;

        #region Methods
        protected override void OnAttachedTo(ContentPage bindable)
        {
            listView = bindable.FindByName<Syncfusion.ListView.XForms.SfListView>("FavoriteDocumentListView");

            viewModel = new FavoritesDocumentViewModel();
            viewModel.sfListView = listView;
            listView.BindingContext = viewModel;
            listView.ItemsSource = FavoritesDocumentViewModel.FavoritesDocuments;
            listView.ItemGenerator = new ItemGeneratorExt(listView);
            listView.SwipeStarted += ListView_SwipeStarted;

            base.OnAttachedTo(bindable);
        }

        protected override void SwipeButton_Clicked(int itemIndex)
        {
            if (Swipped == false)
            {
                listView.SwipeItem(FavoritesDocumentViewModel.FavoritesDocuments[itemIndex], -180);
                viewModel.ItemToRemove = FavoritesDocumentViewModel.FavoritesDocuments.ToList().First(item => item.ItemIndexInFavoriteDocumentList == itemIndex);
                Swipped = true;
            }
            else
            {
                listView.ResetSwipe();
                Swipped = false;
            }
        }

        protected override void LoadPropertiesFile(int itemIndex, string password = null, bool isFirstTry = false)
        {
            CloseComponents();

            using (Stream docStream = new FileStream(FavoritesDocumentViewModel.FavoritesDocuments[itemIndex].FilePath, FileMode.Open))
            {
                try
                {
                    using (var document = password == null
                        ? new PdfLoadedDocument(docStream)
                        : new PdfLoadedDocument(docStream, password))
                    {
                        pdfPropertyPopup.PdfPropertyListView.ItemsSource = LoadPropertiesInListView(document);

                        getInfoFilePopup.IsOpen = true;
                    }
                }
                catch (PdfDocumentException exception)
                {
                    if (exception.Message == "Can't open an encrypted document. The password is invalid.")
                    {
                        if (isFirstTry)
                        {
                            passwordPopup.Show();
                        }
                        else
                        {
                            label.Text = "The password is incorrect. Please try again";
                            passwordPopup.Show();
                        }
                    }
                }
            }
        }

        protected override IList<DocumentInfoListViewModel> LoadPropertiesInListView(PdfLoadedDocument document)
        {
            IList<DocumentInfoListViewModel> documentInfoListViewModels = new List<DocumentInfoListViewModel>
            {
                new DocumentInfoListViewModel(label: "Filename", labelResult: (viewModel.ItemToRemove.FileName)),
                new DocumentInfoListViewModel(label: "Pages numbers", labelResult: document.PageCount.ToString()),
                new DocumentInfoListViewModel(label: "Author", labelResult: document.DocumentInformation.Author),
                new DocumentInfoListViewModel(label: "Creation date",
                    labelResult: document.DocumentInformation.CreationDate.ToString()),
                new DocumentInfoListViewModel(label: "Creator", labelResult: document.DocumentInformation.Creator),
                new DocumentInfoListViewModel(label: "Keywords", labelResult: document.DocumentInformation.Keywords),
                new DocumentInfoListViewModel(label: "Subject", labelResult: document.DocumentInformation.Subject),
                new DocumentInfoListViewModel(label: "Title", labelResult: document.DocumentInformation.Title),
                new DocumentInfoListViewModel(label: "Last modification",
                    labelResult: document.DocumentInformation.ModificationDate.ToString()),
                new DocumentInfoListViewModel(label: "Creation date",
                    labelResult: (viewModel.ItemToRemove.CreationTime.ToString())),
                new DocumentInfoListViewModel(label: "Producer", labelResult: document.DocumentInformation.Producer),
                new DocumentInfoListViewModel(label: "File size", labelResult: (viewModel.ItemToRemove.Size)),
                new DocumentInfoListViewModel(label: "File path", labelResult: (viewModel.ItemToRemove.FilePath))
            };
            return documentInfoListViewModels;
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            viewModel = null;

            base.OnDetachingFrom(bindable);
        }
        #endregion
    }
}
