using Pdf.Helpers;
using Pdf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Models
{
    public class ProgressItem : BaseViewModel
    {
        double progressPercentage = 0;
        Progress<DownloadBytesProgress> progress;

        public double ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }

            set
            {
                
                SetProperty(ref progressPercentage, value);
            }
        }

        public Progress<DownloadBytesProgress> Progress
        {
            get
            {
                return progress;
            }
            set
            {
                SetProperty(ref progress, value);
            }
        }

        public ProgressItem(Progress<DownloadBytesProgress> progress)
        {
            this.progress = progress;
        }

        public void UpdateProgress(double progressBarPercentage)
        {
            progressPercentage = progressBarPercentage;
        }

    }
}
