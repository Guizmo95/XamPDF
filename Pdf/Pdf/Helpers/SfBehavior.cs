using Pdf.controls;
using Pdf.Enumerations;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.ListView.XForms;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Unity;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class SfBehavior : Behavior<ContentPage>, INotifyPropertyChanged
    {
        #region Fields
        private Syncfusion.ListView.XForms.SfListView ListView;
        private Command<int> swipeButtonCommand;
        private Command<int> infoDocumentCommand;

        private SfPopupLayout getInfoFilePopup;
        private PdfPropertyPopup pdfPropertyPopup;
        private SfPopupLayout passwordPopup;

        private DataTemplate templateViewSetPasswordPopup;

        Label label;
        Entry entry;
        Button acceptButton;
        Button declineButton;

        private DocumentViewModel viewModel;
        private SearchBar searchBar = null;

        private bool isSwipped;
        private int itemIndex;

        private BehaviorType behaviorType;
        #endregion

        public bool IsSwipped
        {
            get
            {
                return isSwipped;
            }
            set
            {
                isSwipped = value;
                OnPropertyChanged();
            }
        }

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
            ListView.ItemGenerator = new ItemGeneratorExt(this.ListView);
            ListView.SwipeStarted += ListView_SwipeStarted;

            SwipeButtonCommand = new Command<int>((int itemIndex) => SwipeButton_Clicked(itemIndex));
            InfoDocumentCommand = new Command<int>((int itemIndex) => GetInfoDocument(itemIndex));

            searchBar = bindable.FindByName<SearchBar>("filterDocument");
            searchBar.TextChanged += SearchBar_TextChanged;

            base.OnAttachedTo(bindable);

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

            passwordPopup = new SfPopupLayout();
            passwordPopup.PopupView.HeightRequest = 200;
            passwordPopup.PopupView.WidthRequest = 310;
            passwordPopup.PopupView.ShowHeader = true;
            passwordPopup.PopupView.ShowFooter = true;
            passwordPopup.PopupView.ShowCloseButton = true;
            passwordPopup.PopupView.AnimationDuration = 170;
            passwordPopup.PopupView.HeaderHeight = 63;
            passwordPopup.Closed += PasswordPopup_Closed;

            DataTemplate templateViewHeaderSetPasswordPopop = new DataTemplate(() =>
            {
                Label label = new Label();
                label.VerticalTextAlignment = TextAlignment.Center;
                label.Padding = new Thickness(20,0,20,0);
                label.Text = "Password";
                label.TextColor = Color.Black;
                label.FontSize = 17;
                label.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";

                return label;
            });


            templateViewSetPasswordPopup = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();

                label = new Label();

                label.Text = "This file is password protected. Please enter the password";
                label.TextColor = Color.Black;
                label.FontSize = 13.5;
                label.Padding = new Thickness(20, 0, 20, 0);
                label.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";

                entry = new Entry();
                entry.FontSize = 13.5;
                entry.Margin = new Thickness(19, 0, 19, 0);
                entry.IsPassword = true;
                entry.Placeholder = "Enter the password";
                entry.TextColor = Color.Black;

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(entry);

                return stackLayout;
            });

            DataTemplate footerTemplateViewSetPasswordPopup = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Horizontal;
                stackLayout.Spacing = 0;

                acceptButton = new Button();
                acceptButton.Text = "Ok";
                acceptButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                acceptButton.FontSize = 14;
                acceptButton.TextColor = Color.Black;
                acceptButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                acceptButton.BackgroundColor = Color.White;
                acceptButton.VerticalOptions = LayoutOptions.Center;
                acceptButton.WidthRequest = 86;
                acceptButton.Clicked += AcceptButtonPasswordPopup_Clicked;
                declineButton = new Button();
                declineButton.Text = "Cancel";
                declineButton.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                declineButton.FontSize = 14;
                declineButton.TextColor = Color.Black;
                declineButton.HorizontalOptions = LayoutOptions.End;
                declineButton.BackgroundColor = Color.White;
                declineButton.VerticalOptions = LayoutOptions.Center;
                declineButton.WidthRequest = 89;
                declineButton.Clicked += DeclineButton_Clicked;

                stackLayout.Children.Add(acceptButton);
                stackLayout.Children.Add(declineButton);

                return stackLayout;
            });

            // Adding ContentTemplate of the SfPopupLayout
            passwordPopup.PopupView.ContentTemplate = templateViewSetPasswordPopup;

            // Adding FooterTemplate of the SfPopupLayout
            passwordPopup.PopupView.FooterTemplate = footerTemplateViewSetPasswordPopup;

            // Adding FooterTemplate of the SfPopupLayout
            passwordPopup.PopupView.HeaderTemplate = templateViewHeaderSetPasswordPopop;
            
        }



        //TODO -- CLOSE KEYBOARD

        private void PasswordPopup_Closed(object sender, EventArgs e)
        {
            label.Text = "This file is password protected. Please enter the password";

            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }

        private void DeclineButton_Clicked(object sender, EventArgs e)
        {
            passwordPopup.IsOpen = false;
        }

        private void AcceptButtonPasswordPopup_Clicked(object sender, EventArgs e)
        {
            OpenFileToShowInfo(itemIndex, entry.Text, false);
        }

        private void GetInfoFilePopup_Closing(object sender, Syncfusion.XForms.Core.CancelEventArgs e)
        {
            ListView.ResetSwipe();
            this.IsSwipped = false;

            DependencyService.Get<INavBarHelper>().SetDefaultNavBar();
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            ListView = null;
            viewModel = null;
            getInfoFilePopup = null;
            pdfPropertyPopup = null;
            passwordPopup = null;
            templateViewSetPasswordPopup = null;

            searchBar = null;
            searchBar.TextChanged -= SearchBar_TextChanged;
            getInfoFilePopup.Closing -= GetInfoFilePopup_Closing;
            ListView.SwipeStarted -= ListView_SwipeStarted;
            declineButton.Clicked -= DeclineButton_Clicked;
            acceptButton.Clicked -= AcceptButtonPasswordPopup_Clicked;
            passwordPopup.Closed -= PasswordPopup_Closed;
            base.OnDetachingFrom(bindable);
        }

        private void GetInfoDocument(int itemIndex)
        {
            OpenFileToShowInfo(itemIndex, null, true);
            this.itemIndex = itemIndex;
        }

        private void OpenFileToShowInfo(int itemIndex, string password = null, bool isFirstTry = false)
        {
            using (Stream docStream = new FileStream(DocumentViewModel.Documents[itemIndex].FilePath, FileMode.Open))
            {
                try
                {
                    PdfLoadedDocument document;

                    if (password == null)
                    {
                        document = new PdfLoadedDocument(docStream);
                    }
                    else
                    {
                        document = new PdfLoadedDocument(docStream, password);
                    }

                    DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                    DependencyService.Get<INavBarHelper>().SetImmersiveMode();

                    ListView.ResetSwipe();
                    this.IsSwipped = false;

                    passwordPopup.IsOpen = false;

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

                    document.Close();
                }
                catch (PdfDocumentException exception)
                {
                    if (exception.Message == "Can't open an encrypted document. The password is invalid.")
                    {
                        if(isFirstTry == true)
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

        private void SwipeButton_Clicked(int itemIndex)
        {
            if(this.IsSwipped == false)
            {
                ListView.SwipeItem(DocumentViewModel.Documents[itemIndex], -180);
                viewModel.ItemToRemove = DocumentViewModel.Documents.ToList().First(item => item.ItemIndexInDocumentList == itemIndex);
                this.IsSwipped = true;
            }
            else
            {
                ListView.ResetSwipe();
                this.IsSwipped = false;
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        #endregion
    }
}
