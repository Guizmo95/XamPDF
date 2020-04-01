using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Android.OS;
using Pdf.Droid;
using Pdf.Droid.Helpers;
using Pdf.Interfaces;
using Android.Graphics.Pdf;
using System.Diagnostics;
using System.Threading.Tasks;
using static Android.Graphics.Bitmap;
using Xamarin.Forms;
using Pdf.Models;
using System.Linq;
using Xamarin.Forms.Internals;

[assembly: Xamarin.Forms.Dependency(typeof(GetThumbnails))]
namespace Pdf.Droid.Helpers
{
    public class GetThumbnails : IGetThumbnails
    {

        List<ThumbnailsModel> items = new List<ThumbnailsModel>();
        bool inProccess = false;

        public bool InProccess
        {
            get
            {
                return inProccess;
            }

            set
            {
                inProccess = value;
            }
        }

        public List<ThumbnailsModel> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        //TODO -- DELETE REPOSITORY AFTER PROCCESS                               

        public async Task<List<ThumbnailsModel>> GetBitmaps(string filePath, int lastIndex = 0, int numberOfItemsPerPage = 10)
        {
            var sw = new Stopwatch();
            sw.Start();

            PdfRenderer pdfRenderer = new PdfRenderer(GetSeekableFileDescriptor(filePath));

            //CHECK PATH LATER
            var appDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string directoryPath = System.IO.Path.Combine(appDirectory, "thumbnailsTemp", System.IO.Path.GetFileNameWithoutExtension(fileName));
            //List<ThumbnailsModel> thumbnailsModels = new List<ThumbnailsModel>();

            if (InProccess == false)
            {
                Directory.CreateDirectory(directoryPath);
                InProccess = true;
            }

            //int pageCount = pdfRenderer.PageCount;
            for (int i = lastIndex; i < lastIndex + numberOfItemsPerPage; i++)
            {
                PdfRenderer.Page page = pdfRenderer.OpenPage(i);

                Android.Graphics.Bitmap bmp = Android.Graphics.Bitmap.CreateBitmap(page.Width, page.Height, Android.Graphics.Bitmap.Config.Argb8888);
                page.Render(bmp, null, null, PdfRenderMode.ForDisplay);

                try
                {
                    using (FileStream output = new FileStream(System.IO.Path.Combine(directoryPath, fileName + "Thumbnails" + i + ".png"), FileMode.Create))
                    {
                        bmp.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, output);
                    }

                    page.Close();
                }
                catch (Exception ex)
                {
                    //TODO -- GERER CETTE EXPEXPTION
                    throw new Exception();
                }
            }


            int y = lastIndex + 1;
            Directory.GetFiles(directoryPath).ToList<string>().Skip(lastIndex).Take(numberOfItemsPerPage).ForEach(delegate (string thumbnailsEmplacement)
            {
                Items.Add(new ThumbnailsModel(y, thumbnailsEmplacement));
                y++;
            });

            sw.Stop();

            return await Task.FromResult(Items);
        }

        public async Task<IEnumerable<ThumbnailsModel>> GetItemsAsync(string filePath, bool forceRefresh = false, int lastIndex = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.01));

            Dictionary<bool, int> keyValuePairs = IsUnder15Pages(filePath);

            if (keyValuePairs.ContainsKey(true))
            {
                await GetBitmaps(filePath, lastIndex, keyValuePairs[true]);
                return await Task.FromResult(Items);
            }
            else
            {
                int numberOfItemsPerPage = 10;
                await GetBitmaps(filePath, lastIndex, numberOfItemsPerPage);
                return await Task.FromResult(Items.Skip(lastIndex).Take(numberOfItemsPerPage));
            }


        }

        public Dictionary<bool, int> IsUnder15Pages(string filePath)
        {
            Dictionary<bool, int> keyValuePairs = new Dictionary<bool, int>();
            bool response = false;

            PdfRenderer pdfRenderer = new PdfRenderer(GetSeekableFileDescriptor(filePath));

            if (pdfRenderer.PageCount < 10)
            {
                response = true;
            }
            else
            {
                response = false;
            }

            keyValuePairs.Add(response, pdfRenderer.PageCount);
            return keyValuePairs;
        }

        public ParcelFileDescriptor GetSeekableFileDescriptor(string filePath)
        {
            ParcelFileDescriptor fileDescriptor = null;

            try
            {
                fileDescriptor = ParcelFileDescriptor.Open(new Java.IO.File(filePath), ParcelFileMode.ReadOnly);
            }
            catch (Exception ex)
            {
                //TODO Handle exeption
                throw;
            }

            return fileDescriptor;
        }

        public int GetAllPages(string filePath)
        { 
            PdfRenderer pdfRenderer = new PdfRenderer(GetSeekableFileDescriptor(filePath));

            int pagesCount = pdfRenderer.PageCount;

            return pagesCount;
        }

        public void DeleteThumbnailsRepository(string filePath)
        {
            var appDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

            string directoryPath = System.IO.Path.Combine(appDirectory, "thumbnailsTemp", System.IO.Path.GetFileNameWithoutExtension(fileName));

            File.Delete(directoryPath);
        }
    }
}