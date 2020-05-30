using Android.Media;
using Pdf.Models;
using Syncfusion.ListView.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupMenuContent : ContentView
    {
        public IList ItemsMenu { get; set; }

        public SfListView MenuListView
        {
            get
            {
                return menuListView;
            }
        }
        public PopupMenuContent()
        {
            InitializeComponent();

            ItemsMenu = new List<ItemsMenu>();

            ItemsMenu.Add(new ItemsMenu
            {
                Id = 0,
                Image = "baseline_save_24.xml",
                TextOption = "Save PDF",
            });

            ItemsMenu.Add(new ItemsMenu
            {
                Id = 1,
                Image = "baseline_print_24.xml",
                TextOption = "Print PDF",
            });

            menuListView.ItemsSource = ItemsMenu;
        }

    }
}