using Pdf.Interfaces;
using Pdf.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private readonly IAndroidFileHelper androidFileHelper;
        private Command<int> favoritesImageCommand;

        public static ObservableCollection<FileModel> Documents { get; set; }
        public Command<int> FavoritesImageCommand
        {
            get => favoritesImageCommand;
            protected set => SetProperty(ref favoritesImageCommand, value);
        }

        public DocumentViewModel()
        {
            androidFileHelper = DependencyService.Get<IAndroidFileHelper>();

            Documents = new ObservableCollection<FileModel>();
            FavoritesDocumentViewModel.FavoritesDocuments = new ObservableCollection<FileModel>(App.Database.GetItemsAsync().Result);

            LoadListView();

            FavoritesImageCommand = new Command<int>((int itemIndex) => SetFavorites(itemIndex));
        }

        private void LoadListView()
        {
            var i = 0;
            androidFileHelper.GetPdfFiles().ForEach(delegate(FileInfo fileInfo)
            {
                var fileModel = new FileModel()
                {
                    ItemIndexInDocumentList = i,
                    FileName = fileInfo.Name,
                    CreationTime = fileInfo.CreationTime.Date,
                    FileLenght = fileInfo.Length,
                    FilePath = fileInfo.FullName,
                };

                fileModel.IsFavorite = FavoritesDocumentViewModel.FavoritesDocuments.Any(x => x.FilePath == fileInfo.FullName) == true;

                fileModel.GetHumanReadableFileSize();

                Documents.Add(fileModel);

                i++;
            });
        }

        protected override void DeleteFileButton_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(filePathToDelete) != true)
            {
                File.Delete(filePathToDelete);

                if (ItemToRemove != null)
                {
                    Documents.Remove(ItemToRemove);

                    DecreaseItemIndexForEachPdfFiles();
                }

                deleteFilePopup.IsOpen = false;
            }

            sfListView.ResetSwipe();
        }

        protected override void DecreaseItemIndexForEachPdfFiles()
        {
            Documents.ToList().ForEach(delegate(FileModel fileModel)
            {
                if (fileModel.ItemIndexInDocumentList > ItemToRemove.ItemIndexInDocumentList)
                {
                    fileModel.ItemIndexInDocumentList -= 1;
                }
            });

            if (FavoritesDocumentViewModel.FavoritesDocuments.Contains(ItemToRemove))
            {
                FavoritesDocumentViewModel.FavoritesDocuments.Remove(ItemToRemove);

                FavoritesDocumentViewModel.FavoritesDocuments.ToList().ForEach(delegate(FileModel fileModel)
                {
                    if (fileModel.ItemIndexInFavoriteDocumentList > ItemToRemove.ItemIndexInFavoriteDocumentList)
                    {
                        fileModel.ItemIndexInFavoriteDocumentList -= 1;
                    }
                });
            }
        }

        public void SetFavorites(int itemIndex)
        {
            if (itemIndex >= 0)
            {
                var item = Documents[itemIndex];
                if (item.IsFavorite)
                    RemoveFromFavorite(item);
                else
                {
                    AddFileInFavorite(item);

                    DependencyService.Get<IToastMessage>().ShortAlert("File added to favorites");
                }
            }
            sfListView.ResetSwipe();
        }

        private void RemoveFromFavorite(FileModel item)
        {
            Documents[Documents.IndexOf(item)].IsFavorite = false;
            App.Database.DeleteItemAsync(item);
            FavoritesDocumentViewModel.FavoritesDocuments.Remove(item);
            DependencyService.Get<IToastMessage>().ShortAlert("File removed from favorites");
        }

        private static void AddFileInFavorite(FileModel item)
        {
            App.Database.SaveItemAsync(item);

            Documents[Documents.IndexOf(item)].IsFavorite = true;
            item.IsFavorite = true;

            if (FavoritesDocumentViewModel.FavoritesDocuments.Count != 0)
                item.ItemIndexInFavoriteDocumentList += 1;
            else if (FavoritesDocumentViewModel.FavoritesDocuments.Count == 0)
                item.ItemIndexInFavoriteDocumentList = 0;

            FavoritesDocumentViewModel.FavoritesDocuments.Add(item);
        }
    }
}

