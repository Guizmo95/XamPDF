using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdfClient.Models
{
    public class SummaryModel
    {
        private string title;
        private int pageNumber;

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

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public SummaryModel(string title, int pageNumber)
        {
            this.title = title;
            this.pageNumber = pageNumber;
        }
    }
}