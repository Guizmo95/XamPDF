using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class DocumentInfoListViewModel
    {
        private string label;
        private string labelResult;

        public string Label
        {
            get
            {
                return label;
            }

            set
            {
                label = value;
            }
        }

        public string LabelResult
        {
            get
            {
                return labelResult;
            }

            set
            {
                labelResult = value;
            }
        }

        public DocumentInfoListViewModel(string label, string labelResult)
        {
            this.Label = label;
            this.LabelResult = labelResult;
        }
    }
}
