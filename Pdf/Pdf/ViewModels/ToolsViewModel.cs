using Pdf.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pdf.ViewModels
{
    public class ToolsViewModel
    {
        public IList<ToolsCustomItem> ToolsItems { get; set; }

        public ToolsViewModel()
        {
            List<ToolsCustomItem> toolsCustomItems = new List<ToolsCustomItem>();
            toolsCustomItems.Add(new ToolsCustomItem(0, "Working with pages", "sharp_layers_24.xml"));
            toolsCustomItems.Add(new ToolsCustomItem(1, "Working with Documents", "baseline_description_24.xml"));

            ToolsItems = toolsCustomItems;
        }
    }
}
