﻿using Acr.UserDialogs;
using Pdf.controls;
using Pdf.Helpers;
using Pdf.Models;
using Pdf.ViewModels;
using Syncfusion.DataSource;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DocumentsList : ContentPage, INotifyPropertyChanged
    {
        private SfPopupLayout popupLayout;

        public SfBehavior SfBehavior { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DocumentsList()
        {
            InitializeComponent();

            SfBehavior = new SfBehavior();
            popupLayout = new SfPopupLayout();
            popupLayout.PopupView.HeightRequest = 92;
            popupLayout.PopupView.WidthRequest = 83;
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;
            popupLayout.PopupView.ShowCloseButton = false;
            popupLayout.PopupView.AnimationMode = AnimationMode.Fade;
            popupLayout.PopupView.AnimationEasing = AnimationEasing.SinOut;
            popupLayout.PopupView.AnimationDuration = 150;

            DataTemplate templateView = new DataTemplate(() =>
            {
                StackLayout stackLayout = new StackLayout();
                stackLayout.Orientation = StackOrientation.Vertical;
                stackLayout.VerticalOptions = LayoutOptions.Center;
                stackLayout.Spacing = 0;

                Button sortName;
                sortName = new Button();
                sortName.HeightRequest = 30;
                sortName.WidthRequest = 83;
                sortName.Text = "Sort name";
                sortName.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortName.BackgroundColor = Color.White;
                sortName.FontSize = 7.7;
                sortName.Clicked += SortName_Clicked;

                Button sortDate;
                sortDate = new Button();
                sortDate.HeightRequest = 30;
                sortDate.WidthRequest = 83;
                sortDate.Text = "Sort date";
                sortDate.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortDate.BackgroundColor = Color.White;
                sortDate.FontSize = 7.7;
                sortDate.Clicked += SortDate_Clicked;

                Button sortSize;
                sortSize = new Button();
                sortSize.HeightRequest = 30;
                sortSize.WidthRequest = 83;
                sortSize.Text = "Sort size";
                sortSize.FontFamily = "GothamMedium_1.ttf#GothamMedium_1";
                sortSize.BackgroundColor = Color.White;
                sortSize.FontSize = 7.7;
                sortSize.Clicked += SortSize_Clicked;

                stackLayout.Children.Add(sortName);
                stackLayout.Children.Add(sortDate);
                stackLayout.Children.Add(sortSize);

                return stackLayout;
            });

            popupLayout.PopupView.ContentTemplate = templateView;

            behavior.BindingContext = SfBehavior;

            SortName_Clicked(null, null);

            sortButton.RotateTo(180);
        }


        void HamburgerButton_Clicked(object sender, EventArgs e)
        {
            if (Shell.Current.FlyoutIsPresented == true)
                Shell.Current.FlyoutIsPresented = false;
            else
                Shell.Current.FlyoutIsPresented = true;
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


        private async void DocumentListView_SelectionChanging(object sender, Syncfusion.ListView.XForms.ItemSelectionChangingEventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Loading ...", MaskType.Black);
            try
            {
                if (e.AddedItems == null)
                {
                    return;
                }

                var file = (FileModel)e.AddedItems[0];

                await Navigation.PushAsync(new PdfViewer(file.FilePath));
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void SortButton_Clicked(object sender, EventArgs e)
        {
            popupLayout.Show(sortButton.X + 25, sortButton.Y + 14);
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}