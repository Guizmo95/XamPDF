using Pdf.Data;
using Pdf.Interfaces;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity;
using Xamarin.Forms;

namespace Pdf.Controls
{
    public class DocumentSearchHandler: SearchHandler
    {
        public IList<FileInfo> Documents { get; set; }

        public DocumentSearchHandler()
        {
        }
        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = App.Container.Resolve<IDocumentsData>().Documents;
            }
        }

        protected override async void OnItemSelected(object item)
        {
            //base.OnItemSelected(item);

            //var fileInfo = (FileInfo)item;

            //using (Stream stream = File.OpenRead(fileInfo.FullName))
            //{
            //    await 
            //}
        }
    }
}
