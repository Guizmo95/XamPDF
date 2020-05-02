using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.ListView.XForms;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Unity;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class SfBehavior : Behavior<ContentPage>
    {

        #region Fields
        private Syncfusion.ListView.XForms.SfListView ListView;
        private Command<int> swipeButtonCommand;
        private Command<int> infoDocumentCommand;

        private SfPopupLayout getInfoFilePopup;
        private PdfPropertyPopup pdfPropertyPopup;

        private DocumentViewModel viewModel;
        private SearchBar searchBar = null;

        private bool isSwipped;

        private BehaviorType behaviorType;

        #endregion

        public Command<int> SwipeButtonCommand
        {
            get { return swipeButtonCommand; }
            protected set { swipeButtonCommand = value; }
        }
        public Command<int> InfoDocumentCommand
        {
            get { return infoDocumentCommand; }
            protected set { infoDocumentCommand = value; }
        }

        public DocumentViewModel DocumentViewModel
        {
            get { return viewModel; }
            protected set { viewModel = value; }
        }

        #region Methods
        protected override void OnAttachedTo(ContentPage bindable)
        {

            ListView = bindable.FindByName<Syncfusion.ListView.XForms.SfListView>("DocumentListView");

            viewModel = new DocumentViewModel();
            viewModel.sfListView = ListView;
            ListView.BindingContext = viewModel;
            ListView.ItemsSource = DocumentViewModel.Documents;
            ListView.SwipeStarted += ListView_SwipeStarted;

            SwipeButtonCommand = new Command<int>((int itemIndex) => SwipeButton_Clicked(itemIndex));
            InfoDocumentCommand = new Command<int>((int itemIndex) => GetInfoDocument(itemIndex));

            searchBar = bindable.FindByName<SearchBar>("filterDocument");
            searchBar.TextChanged += SearchBar_TextChanged;

            base.OnAttachedTo(bindable);

            getInfoFilePopup = new SfPopupLayout();
            getInfoFilePopup.PopupView.IsFullScreen = true;
            getInfoFilePopup.PopupView.AnimationDuration = 200;
            getInfoFilePopup.PopupView.AnimationMode = AnimationMode.SlideOnTop;
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
        }


        private void GetInfoFilePopup_Closing(object sender, Syncfusion.XForms.Core.CancelEventArgs e)
        {
            ListView.ResetSwipe();
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            ListView = null;
            viewModel = null;
            getInfoFilePopup = null;
            pdfPropertyPopup = null;

            searchBar = null;
            searchBar.TextChanged -= SearchBar_TextChanged;
            getInfoFilePopup.Closing -= GetInfoFilePopup_Closing;
            ListView.SwipeStarted -= ListView_SwipeStarted;
            base.OnDetachingFrom(bindable);
        }

        private void GetInfoDocument(int itemIndex)
        {
            using (Stream docStream = new FileStream(DocumentViewModel.Documents[itemIndex].FilePath, FileMode.Open))
            {
                using (PdfLoadedDocument document = new PdfLoadedDocument(docStream))
                {
                    IList<DocumentInfoListViewModel> documentInfoListViewModels = new List<DocumentInfoListViewModel>();
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Filename", (viewModel.ItemToRemove.FileName)));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Pages numbers", document.PageCount.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Author", document.DocumentInformation.Author));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creation date", document.DocumentInformation.CreationDate.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creator", document.DocumentInformation.Creator));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Keywords", document.DocumentInformation.Keywords));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Subject", document.DocumentInformation.Subject));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Title", document.DocumentInformation.Title));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Last modification", document.DocumentInformation.ModificationDate.ToString()));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Creation date", (viewModel.ItemToRemove.CreationTime.ToString())));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("Producer", document.DocumentInformation.Producer));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("File size", (viewModel.ItemToRemove.Size)));
                    documentInfoListViewModels.Add(new DocumentInfoListViewModel("File path", (viewModel.ItemToRemove.FilePath)));

                    pdfPropertyPopup.PdfPropertyListView.ItemsSource = documentInfoListViewModels;

                    getInfoFilePopup.IsOpen = true;
                }
            }
        }

        private void SwipeButton_Clicked(int itemIndex)
        {
            if(this.isSwipped == false)
            {
                ListView.SwipeItem(DocumentViewModel.Documents[itemIndex], -180);
                viewModel.ItemToRemove = DocumentViewModel.Documents.ToList().First(item => item.ItemIndexInDocumentList == itemIndex);
                this.isSwipped = true;
            }
            else
            {
                ListView.ResetSwipe();
                this.isSwipped = false;
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (ListView.DataSource != null)
            {
                ListView.DataSource.Filter = FilterDocument;
                ListView.DataSource.RefreshFilter();
            }
            ListView.RefreshView();
        }

        private bool FilterDocument(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var document = obj as FileModel;
            return (document.FileName.ToLower().Contains(searchBar.Text.ToLower()));
        }

        private void ListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion
    }
}
