using Pdf.Interfaces;
using Pdf.Models;
using Pdf.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        public IList<FileModel> Documents { get; set; }
        private Command favoritesImageCommand;
        private Command deleteImageCommand;
   
        private readonly IPdfPickerAndroid pdfPickerAndroid;
        internal SfListView sfListView;

        internal int ItemIndex
        {
            get;
            set;
        }

        public Command FavoritesImageCommand
        {
            get { return favoritesImageCommand; }
            protected set { favoritesImageCommand = value; }
        }

        public Command DeleteImageCommand
        {
            get { return deleteImageCommand; }
            protected set { deleteImageCommand = value; }
        }



        public DocumentViewModel()
        {
            pdfPickerAndroid = DependencyService.Get<IPdfPickerAndroid>();

            Documents = new List<FileModel>();

            IList<FileModel> fileModels = new List<FileModel>();

            int i = 0;
            pdfPickerAndroid.GetPdfFilesInDocuments().ForEach(delegate (FileInfo fileInfo)
            {
                Documents.Add(new FileModel(i, fileInfo.Name, fileInfo.CreationTime.Date, fileInfo.Length, fileInfo.FullName));
                i++;
            });

            deleteImageCommand = new Command(Delete);
            //favoritesImageCommand = new Command(SetFavorites);

        }

        private void Delete()
        {
            App.Current.MainPage.DisplayAlert("Deleted!", "Item successfully deleted", "OK");
            if (ItemIndex >= 0)
                Documents.RemoveAt(ItemIndex);
            sfListView.ResetSwipe();
        }

        //private void SetFavorites()
        //{
        //    if (ItemIndex >= 0)
        //    {
        //        var item = Documents[ItemIndex];
        //        item.IsFavorite = !item.IsFavorite;
        //    }
        //    sfListView.ResetSwipe();
        //}




    }

    }
