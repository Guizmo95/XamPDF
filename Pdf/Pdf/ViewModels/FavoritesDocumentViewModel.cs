using Pdf.Interfaces;
using Pdf.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using static System.String;

namespace Pdf.ViewModels
{
    public class FavoritesDocumentViewModel : BaseViewModel
    {
        private Command<int> deleteFromFavoritesCommand;

        public static ObservableCollection<FileModel> FavoritesDocuments { get; set; }
        public Command<int> DeleteFromFavoritesCommand
        {
            get => deleteFromFavoritesCommand;
            protected set => SetProperty(ref deleteFromFavoritesCommand, value);
        }

        public FavoritesDocumentViewModel()
        {
            FavoritesDocuments = new ObservableCollection<FileModel>(App.Database.GetItemsAsync().Result);
            DeleteFromFavoritesCommand = new Command<int>((int itemIndex) => RemoveFromFavorites(itemIndex));
        }

        private void RemoveFromFavorites(int itemIndex)
        {
            if (itemIndex >= 0)
            {
                var item = FavoritesDocuments[itemIndex];
                if (item.IsFavorite)
                {
                    DocumentViewModel.Documents.
                        First(c => c.FilePath == item.FilePath).IsFavorite = false;

                    FavoritesDocuments.Remove(item);
                    App.Database.DeleteItemAsync(item);
                }
            }
            sfListView.ResetSwipe();

            DependencyService.Get<IToastMessage>().ShortAlert("File removed from favorites");
        }

        protected override void DeleteFileButton_Clicked(object sender, System.EventArgs e)
        {
            if (IsNullOrEmpty(filePathToDelete) != true)
            {
                File.Delete(filePathToDelete);

                if (ItemToRemove != null)
                {
                    FavoritesDocuments.Remove(ItemToRemove);
                    DocumentViewModel.Documents.Remove(ItemToRemove);

                    DecreaseItemIndexForEachPdfFiles();
                }

                deleteFilePopup.IsOpen = false;
            }

            sfListView.ResetSwipe();
        }

        protected override void DecreaseItemIndexForEachPdfFiles()
        {
            FavoritesDocuments.ToList().ForEach(delegate(FileModel fileModel)
            {
                if (fileModel.ItemIndexInFavoriteDocumentList > ItemToRemove.ItemIndexInFavoriteDocumentList)
                {
                    fileModel.ItemIndexInFavoriteDocumentList -= 1;
                }
            });

            DocumentViewModel.Documents.ToList().ForEach(delegate(FileModel fileModel)
            {
                if (fileModel.ItemIndexInDocumentList > ItemToRemove.ItemIndexInDocumentList)
                {
                    fileModel.ItemIndexInDocumentList -= 1;
                }
            });
        }
    }
}
