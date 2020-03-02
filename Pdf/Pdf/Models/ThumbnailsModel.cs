using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class ThumbnailsModel
    {
        private int pageNumber;
        private string emplacementThumbnails;

        public ThumbnailsModel(int pageNumber, string emplacementThumbnails)
        {
            this.pageNumber = pageNumber;
            this.emplacementThumbnails = emplacementThumbnails;
        }

        public int PageNumber
        {
            get
            {
                return pageNumber;
            }

            set
            {
                pageNumber = value;
            }
        }

        public string EmplacementThumbnails
        {
            get
            {
                return emplacementThumbnails;
            }

            set
            {
                emplacementThumbnails = value;
            }
        }
    }
}
