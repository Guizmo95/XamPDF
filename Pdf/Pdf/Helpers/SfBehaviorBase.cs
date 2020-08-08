using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Pdf.controls;
using Pdf.Interfaces;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public abstract class SfBehaviorBase: Behavior<ContentPage>, INotifyPropertyChanged
    {
        #region Fields
        protected Syncfusion.ListView.XForms.SfListView listView;

        protected SfPopupLayout getInfoFilePopup;
        protected PdfPropertyPopup pdfPropertyPopup;
        protected SfPopupLayout passwordPopup;
        protected DataTemplate templateViewSetPasswordPopup;

        protected ImageButton image;
        protected Label label;
        protected Entry entry;
        protected Button acceptButton;
        protected Button declineButton;

        protected SearchBar searchBar;
        #endregion

        public bool Swipped { get; set; }
        public int ItemIndex { get; set; }
        public Command<int> SwipeButtonCommand { get; protected set; }
        public Command<int> InfoDocumentCommand { get; protected set; }

        protected override void OnAttachedTo(ContentPage bindable)
        {
            SwipeButtonCommand = new Command<int>(SwipeButton_Clicked);
            InfoDocumentCommand = new Command<int>(GetInfoDocument);

            searchBar = bindable.FindByName<SearchBar>("filterDocument");
            searchBar.TextChanged += SearchBar_TextChanged;

            FilePropertiesPopup();
            PasswordPopup();

            base.OnAttachedTo(bindable);
        }

        protected abstract void SwipeButton_Clicked(int itemIndex);

        private void GetInfoDocument(int itemIndex)
        {
            LoadPropertiesFile(itemIndex, null, true);
            ItemIndex = itemIndex;
        }

        protected abstract void LoadPropertiesFile(int itemIndex, string password = null, bool isFirstTry = false);

        protected void CloseComponents()
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
            listView.ResetSwipe();
            Swipped = false;
            passwordPopup.IsOpen = false;
        }

        protected abstract IList<DocumentInfoListViewModel> LoadPropertiesInListView(PdfLoadedDocument document);

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                listView.DataSource.Filter = FilterDocument;
                listView.DataSource.RefreshFilter();
            }
            listView.RefreshView();
        }

        private void FilePropertiesPopup()
        {
            getInfoFilePopup = new SfPopupLayout
            {
                PopupView =
                {
                    IsFullScreen = true,
                    AnimationDuration = 200,
                    AnimationMode = AnimationMode.SlideOnBottom,
                    PopupStyle = {CornerRadius = 0}
                },
                Padding = new Thickness(15, 15, 15, 15)
            };
            //getInfoFilePopup.PopupView.PopupStyle.BorderThickness = 2;
            getInfoFilePopup.PopupView.PopupStyle.BorderColor = Color.White;
            getInfoFilePopup.PopupView.ShowFooter = false;
            getInfoFilePopup.PopupView.ShowHeader = true;
            getInfoFilePopup.PopupView.ShowCloseButton = false;
            getInfoFilePopup.Closing += GetInfoFilePopup_Closing;
            getInfoFilePopup.PopupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#eeeeee");
            getInfoFilePopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#e0e0e0");

            pdfPropertyPopup = new PdfPropertyPopup();

            var templateViewGetInfoPopup = new DataTemplate(() => pdfPropertyPopup);

            var headerTemplateViewGetInfoPopup = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 0, 13, 0)
                };

                var title = new Label
                {
                    Text = "Properties",
                    FontSize = 18,
                    FontFamily = "GothamBold.ttf#GothamBold",
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Color.FromHex("#4e4e4e"),
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                image = new ImageButton
                {
                    Source = "outlineClearViewer.xml",
                    Aspect = Aspect.AspectFit,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End
                };
                image.Clicked += CloseButtonPropertiesPopup_Clicked;

                stackLayout.Children.Add(title);
                stackLayout.Children.Add(image);

                return stackLayout;
            });


            // Adding ContentTemplate of the SfPopupLayout
            getInfoFilePopup.PopupView.ContentTemplate = templateViewGetInfoPopup;

            // Adding HeaderTemplate of the SfPopupLayout
            getInfoFilePopup.PopupView.HeaderTemplate = headerTemplateViewGetInfoPopup;
        }

        protected void GetInfoFilePopup_Closing(object sender, Syncfusion.XForms.Core.CancelEventArgs e)
        {
            listView.ResetSwipe();
            Swipped = false;
        }

        protected void CloseButtonPropertiesPopup_Clicked(object sender, EventArgs e)
        {
            getInfoFilePopup.IsOpen = false;
        }

        protected void PasswordPopup()
        {
            passwordPopup = new SfPopupLayout
            {
                PopupView =
                {
                    HeightRequest = 200,
                    WidthRequest = 310,
                    ShowHeader = true,
                    ShowFooter = true,
                    ShowCloseButton = true,
                    AnimationDuration = 170,
                    HeaderHeight = 63
                }
            };
            passwordPopup.Closed += PasswordPopup_Closed;

            var popupViewHeaderTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    Padding = new Thickness(20, 0, 20, 0),
                    Text = "Password",
                    TextColor = Color.Black,
                    FontSize = 17,
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1"
                };

                return label;
            });

            templateViewSetPasswordPopup = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout();

                label = new Label
                {
                    Text = "This file is password protected. Please enter the password",
                    TextColor = Color.Black,
                    FontSize = 13.5,
                    Padding = new Thickness(20, 0, 20, 0),
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1"
                };


                entry = new Entry
                {
                    FontSize = 13.5,
                    Margin = new Thickness(19, 0, 19, 0),
                    IsPassword = true,
                    Placeholder = "Enter the password",
                    TextColor = Color.Black
                };

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(entry);

                return stackLayout;
            });

            var footerTemplateViewSetPasswordPopup = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout {Orientation = StackOrientation.Horizontal, Spacing = 0};

                acceptButton = new Button
                {
                    Text = "Ok",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    FontSize = 14,
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 86
                };

                acceptButton.Clicked += AcceptButtonPasswordPopup_Clicked;
                declineButton = new Button
                {
                    Text = "Cancel",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    FontSize = 14,
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.End,
                    BackgroundColor = Color.White,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = 89
                };
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
            passwordPopup.PopupView.HeaderTemplate = popupViewHeaderTemplate;
        }

        protected void PasswordPopup_Closed(object sender, EventArgs e)
        {
            label.Text = "This file is password protected. Please enter the password";

            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }

        protected void AcceptButtonPasswordPopup_Clicked(object sender, EventArgs e)
        {
            LoadPropertiesFile(ItemIndex, entry.Text, false);
        }

        protected void DeclineButton_Clicked(object sender, EventArgs e)
        {
            passwordPopup.IsOpen = false;
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            listView = null;
            getInfoFilePopup = null;
            pdfPropertyPopup = null;
            passwordPopup = null;
            templateViewSetPasswordPopup = null;

            searchBar.TextChanged -= SearchBar_TextChanged;
            getInfoFilePopup.Closing -= GetInfoFilePopup_Closing;
            listView.SwipeStarted -= ListView_SwipeStarted;
            declineButton.Clicked -= DeclineButton_Clicked;
            acceptButton.Clicked -= AcceptButtonPasswordPopup_Clicked;
            passwordPopup.Closed -= PasswordPopup_Closed;
            image.Clicked -= CloseButtonPropertiesPopup_Clicked;

            searchBar = null;

            base.OnDetachingFrom(bindable);
        }

        protected bool FilterDocument(object obj)
        {
            if (searchBar?.Text == null)
                return true;

            return obj is FileModel document &&
                   (document.FileName.ToLower().Contains(searchBar.Text.ToLower()));
        }

        protected void ListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
