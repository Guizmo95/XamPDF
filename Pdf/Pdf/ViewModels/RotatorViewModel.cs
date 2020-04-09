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
        public RotatorViewModel(RotatorMode rotatorMode)
        {
            if(rotatorMode == RotatorMode.ColorPicker)
            {
                //Page 1
                ColorPicker colorPicker1 = new ColorPicker();
                //Page 2
                ColorPicker colorPicker2 = new ColorPicker();

                TestCollection.Add(new RotatorModel(colorPicker1));
                TestCollection.Add(new RotatorModel(colorPicker2));
            }
        }
        private List<RotatorModel> testCollection = new List<RotatorModel>();

        public List<RotatorModel> TestCollection
        {
            get { return testCollection; }
            set { testCollection = value; }
        }
    }
}
