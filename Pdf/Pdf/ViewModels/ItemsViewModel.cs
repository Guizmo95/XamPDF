using Acr.UserDialogs;
using Pdf.Interfaces;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Pdf.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        IGetThumbnails getThumbnails = DependencyService.Get<IGetThumbnails>();
        FileInfo fileInfo;
        public ObservableCollection<ThumbnailsModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command ItemTresholdReachedCommand { get; set; }
        public Command RefreshItemsCommand { get; set; }
        public const string ScrollToPreviousLastItem = "Scroll_ToPrevious";
        private int _itemTreshold;
        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }

        public int ItemTreshold
        {
            get { return _itemTreshold; }
            set { SetProperty(ref _itemTreshold, value); }
        }

        public ItemsViewModel(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            ItemTreshold = 1;
            Title = "Browse";
            Items = new ObservableCollection<ThumbnailsModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTresholdReachedCommand = new Command(async () => await ItemsTresholdReached());
            RefreshItemsCommand = new Command(async () =>
            {
                await ExecuteLoadItemsCommand();
                IsRefreshing = false;
            });
        }

        async Task ItemsTresholdReached()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var items = await getThumbnails.GetItemsAsync(fileInfo.FullName, true, Items.Count);


                foreach (var item in items)
                {
                    Items.Add(item);
                }



                Debug.WriteLine($"{items.Count()} {Items.Count} ");
                if (items.Count() == 0)
                {
                    ItemTreshold = 1;

                    LoadItemsCommand.Execute(null);

                    //Items.Clear;

                    //var previousLastItem = Items[0];
                    // MessagingCenter.Send<object, Item>(this, ScrollToPreviousLastItem, previousLastItem);
                    return;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            //Device.BeginInvokeOnMainThread(() => UserDialogs.Instance.ShowLoading("Loading...", MaskType.Black));

            //await Task.Run(async () =>
            //{
                IsBusy = true;

                try
                {
                    ItemTreshold = 1;
                    Items.Clear();
                    getThumbnails.InProccess = false;
                    getThumbnails.Items.Clear();
                    var items = await getThumbnails.GetItemsAsync(fileInfo.FullName, true);

                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    IsBusy = false;
                }

            //}).ContinueWith(result => Device.BeginInvokeOnMainThread(() =>
            //{

            //    UserDialogs.Instance.HideLoading();
            //})
            //);

        }


    }
}
