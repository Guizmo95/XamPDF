using Pdf.Enumerations;
using Pdf.Models;
using Pdf.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pdf.ViewModels
{
    internal class RotatorViewModel
    {
        public RotatorViewModel()
        {
            //Page 1
            var colorPicker1 = new ColorPicker();
            //Page 2
            //var colorPicker2 = new ColorPicker();

            TestCollection.Add(new Rotator(colorPicker1));
            //TestCollection.Add(new Rotator(colorPicker2));
        }

        public List<Rotator> TestCollection { get; set; } = new List<Rotator>();
    }
}
