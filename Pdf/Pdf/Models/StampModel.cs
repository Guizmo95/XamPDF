using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Models
{
    public class StampModel
    {
        ImageSource image;
        double rotation;

        public ImageSource Image { get => image; set => image = value; }
        public double Rotation { get => rotation; set => rotation = value; }
    }
}
