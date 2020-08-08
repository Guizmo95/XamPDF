using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.Models
{
    public class DocumentInfoListViewModel
    {
        public string Label { get; set; }

        public string LabelResult { get; set; }

        public DocumentInfoListViewModel(string label, string labelResult)
        {
            this.Label = label;
            this.LabelResult = labelResult;
        }
    }
}
