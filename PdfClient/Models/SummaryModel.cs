using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class SummaryModel
    {
        private string title;
        private int pageNumber;
        private int titleLvl;

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

        public int TitleLvl
        {
            get
            {
                return titleLvl;
            }

            set
            {
                titleLvl = value;
            }
        }

        public SummaryModel(string title, int pageNumber, int titleLvl)
        {
            this.title = title;
            this.pageNumber = pageNumber;
            this.titleLvl = titleLvl;
        }
    }
}
