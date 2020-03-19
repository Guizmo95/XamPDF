using Pdf.Interfaces;
using Pdf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Pdf.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSummaryModal : ContentPage
    {
        readonly List<SummaryModel> summaries;
        readonly ThumbnailsModel thumbnailsModel;
        public AddSummaryModal(List<SummaryModel> summaries, ThumbnailsModel thumbnailsModel)
        {
            InitializeComponent();

            this.summaries = summaries;
            this.thumbnailsModel = thumbnailsModel;
        }

        private void AddSummaryButton(object sender, EventArgs e)
        {
            int itemPosition = thumbnailsModel.PageNumber - 1;
            int selectedPickerValue = (int)picker.SelectedItem;

            if (selectedPickerValue == 2 && CanAddTitleLvl2(summaries, itemPosition) == true)
            {
                summaries.Add(new SummaryModel(title.Text, thumbnailsModel.PageNumber, selectedPickerValue));
                Navigation.PopModalAsync();
            }
            else
            {
                if(selectedPickerValue == 1)
                {
                    summaries.Add(new SummaryModel(title.Text, thumbnailsModel.PageNumber, selectedPickerValue));
                    Navigation.PopModalAsync();
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("This summary is not associated with a level 1 summary");
                }
            }
        }

        //TODO -- FINISH THIS
        private bool CanAddTitleLvl2(List<SummaryModel> summaries, SummaryModel summaryModel)
        {
            bool canAddTile = false;
            summaries.Add(summaryModel);
            summaries.Sort((x, y) => x.PageNumber.CompareTo(y.PageNumber));
            if(summaries.Count == 0)
            {
                canAddTile = false;
            }
            else
            {
                if(summaries.GetRange(0, itemPosition).Exists(x => x.TitleLvl == 1) == true)
                {
                    canAddTile = true;
                }
                else
                {
                    canAddTile = false;
                }
            }
            return canAddTile;
        }

    }
}