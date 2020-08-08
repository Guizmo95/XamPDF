﻿using Pdf.Helpers;
using Pdf.Models;
using Syncfusion.DataSource;
using Syncfusion.XForms.PopupLayout;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentsList : ContentPage, INotifyPropertyChanged
    {
        private SfPopupLayout popupLayout;
        public SfBehaviorDocumentsPage SfBehaviorDocumentsPage { get; set; }

        public FileModel SelectedDoc { get; set; }


        public DocumentsList()
        {
            InitializeComponent();

            SfBehaviorDocumentsPage = new SfBehaviorDocumentsPage();
            popupLayout = new SfPopupLayout
            {
                PopupView = {HeightRequest = 124, WidthRequest = 113}, Padding = new Thickness(15, 10, 7, 15)
            };
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;
            popupLayout.PopupView.ShowCloseButton = false;
            popupLayout.PopupView.AnimationMode = AnimationMode.Fade;
            popupLayout.PopupView.AnimationEasing = AnimationEasing.SinOut;
            popupLayout.PopupView.AnimationDuration = 150;

            var templateView = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.Center, Spacing = 0
                };

                var sortName = new Button
                {
                    HeightRequest = 41,
                    WidthRequest = 113,
                    Text = "Sort name",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    BackgroundColor = Color.White,
                    FontSize = 11
                };
                sortName.Clicked += SortName_Clicked;

                var sortDate = new Button
                {
                    HeightRequest = 41,
                    WidthRequest = 113,
                    Text = "Sort date",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    BackgroundColor = Color.White,
                    FontSize = 11
                };
                sortDate.Clicked += SortDate_Clicked;

                var sortSize = new Button
                {
                    HeightRequest = 41,
                    WidthRequest = 113,
                    Text = "Sort size",
                    FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                    BackgroundColor = Color.White,
                    FontSize = 11
                };
                sortSize.Clicked += SortSize_Clicked;

                stackLayout.Children.Add(sortName);
                stackLayout.Children.Add(sortDate);
                stackLayout.Children.Add(sortSize);

                return stackLayout;
            });

            popupLayout.PopupView.ContentTemplate = templateView;

            behavior.BindingContext = SfBehaviorDocumentsPage;

            SortName_Clicked(null, null);

            sortButton.RotateTo(180);

        }

        private void SortDate_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "creationtime",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortName_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "filename",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortSize_Clicked(object sender, EventArgs e)
        {
            DocumentListView.DataSource.SortDescriptors.Clear();
            DocumentListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "FileLenght",
                Direction = Syncfusion.DataSource.ListSortDirection.Ascending,
            });
            DocumentListView.RefreshView();
        }

        private void SortButton_Clicked(object sender, EventArgs e)
        {
            // Shows SfPopupLayout at the top of the button.
            popupLayout.ShowRelativeToView(sortButton, RelativePosition.AlignBottomRight, 0, -20);
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            headerLabel.IsVisible = false;
            searchButton.IsVisible = false;
            sortButton.IsVisible = false;

            sortButton.HorizontalOptions = LayoutOptions.EndAndExpand;

            filterDocument.IsVisible = true;
            clearSearchBar.IsVisible = true;
        }

        private void ClearSearchBar_Clicked(object sender, EventArgs e)
        {
            headerLabel.IsVisible = true;
            searchButton.IsVisible = true;
            sortButton.IsVisible = true;

            sortButton.HorizontalOptions = LayoutOptions.End;

            filterDocument.IsVisible = false;
            clearSearchBar.IsVisible = false;
        }

        //private async void DocumentListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        var file = (FileModel)DocumentListView.SelectedItem;

        //        if (file != null)
        //        {
        //            await Navigation.PushAsync(new PdfViewer(file.FilePath));
        //        }
        //    }

        //    finally
        //    {
        //        DocumentListView.SelectedItem = null;
        //    }
        //}

        private async void DocumentListView_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            //await Task.Delay(350);

            var file = e.ItemData; 

            //if (file != null)
            //{
            //    await Navigation.PushAsync(new PdfViewer(file.FilePath));
            //}

            //DocumentListView.SelectedItem = null;

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //private async void SfEffectsView_AnimationCompleted(object sender, EventArgs e)
        //{
        //    var file = (FileModel)DocumentListView.SelectedItem;

        //    if (file != null)
        //    {
        //        await Navigation.PushAsync(new PdfViewer(file.FilePath));
        //    }

        //    DocumentListView.SelectedItem = null;
        //}


    }
}