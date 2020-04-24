﻿using Pdf.ViewModels;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pdf.Helpers
{
    public class SfBehavior : Behavior<ContentPage>
    {

        #region Fields
        private Syncfusion.ListView.XForms.SfListView ListView;
        private Command<int> swipeButtonCommand;
        private DocumentViewModel viewModel;

        #endregion

        public Command<int> SwipeButtonCommand
        {
            get { return swipeButtonCommand; }
            protected set { swipeButtonCommand = value; }
        }

        public DocumentViewModel DocumentViewModel
        {
            get { return viewModel; }
            protected set { viewModel = value; }
        }

        #region Methods
        protected override void OnAttachedTo(ContentPage bindable)
        {
            ListView = bindable.FindByName<Syncfusion.ListView.XForms.SfListView>("DocumentListView");

            viewModel = new DocumentViewModel();
            viewModel.sfListView = ListView;
            ListView.BindingContext = viewModel;
            ListView.ItemsSource = viewModel.Documents;

            SwipeButtonCommand = new Command<int>((int id) => SwipeButton_Clicked(id));


            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            ListView = null;
            viewModel = null;
            base.OnDetachingFrom(bindable);
        }

        private void SwipeButton_Clicked(int id)
        {
            ListView.SwipeItem(viewModel.Documents[id], -180);
            viewModel.ItemIndex = id;
        }
        #endregion
    }
}
