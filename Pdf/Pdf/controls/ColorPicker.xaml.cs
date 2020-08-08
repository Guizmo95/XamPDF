using Syncfusion.XForms.Border;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    #region Xamarin.Forms Segmented Control as Color Picker

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColorPicker : ContentView, INotifyPropertyChanged
    {
        #region Members

        readonly List<Color> primaryColors = new List<Color>();
        private int selectedIndex = 0;
        private Color selectedColor;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        public SfSegmentedControl SfSegmentedControl => segctrl;

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
                OnPropertyChanged("SelectedColor");
                MessagingCenter.Send<ColorPicker, Color>(this, "selectedColor", primaryColors[selectedIndex]);
            }
        }

        public Color SelectedColor
        {
            get => primaryColors[selectedIndex];
            set
            {
                selectedColor = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public ObservableCollection<object> ViewCollection { get; set; }

        #endregion

        #region On Property Changed

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region Constructor

        public ColorPicker()
        {
            InitializeComponent();

            ViewCollection = new ObservableCollection<object>();
            primaryColors.Add(Color.Black);
            primaryColors.Add(Color.Violet);
            primaryColors.Add(Color.Indigo);
            primaryColors.Add(Color.Blue);
            primaryColors.Add(Color.Green);
            primaryColors.Add(Color.Yellow);

            foreach (var color in primaryColors)
            {
                var grid = new Grid();
                var border = new SfBorder
                {
                    Margin = new Thickness(2),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    CornerRadius = 20,
                    BorderWidth = 0,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    BackgroundColor = color
                };
                grid.Children.Add(border);
                ViewCollection.Add(grid);
            }

            this.BindingContext = this;
        }

        #endregion

        #endregion
    }
}