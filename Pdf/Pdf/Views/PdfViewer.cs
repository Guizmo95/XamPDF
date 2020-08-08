using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Pdf.controls;
using Pdf.Interfaces;
using Pdf.Models;
using Syncfusion.SfPdfViewer.XForms;
using Syncfusion.XForms.PopupLayout;
using Xamarin.Forms;

namespace Pdf.Views
{
    public partial class PdfViewer
    {
        private void InitializeStylePopup()
        {
            styleContent = new StyleContent();
            stylePopup = new SfPopupLayout
            {
                ClosePopupOnBackButtonPressed = false,
                PopupView =
                {
                    ShowHeader = false,
                    ShowFooter = false,
                    HeightRequest = 192,
                    WidthRequest = 280,
                    PopupStyle = {BorderColor = Color.FromHex("#fafafa")},
                    AnimationMode = AnimationMode.SlideOnBottom
                }
            };

            styleTemplateStylePopup = new DataTemplate(() => styleContent);

            stylePopup.PopupView.ContentTemplate = styleTemplateStylePopup;

            styleContent.ThicknessBar.ThicknessBarClicked += ThicknessBar_Clicked;
            styleContent.OpacityButtonClicked += OpacityIcon_Clicked;
        }

        private void ThicknessBar_Clicked(int thicknessBarIndexSelected)
        {
            if (lastThicknessValueSelected != thicknessBarIndexSelected)
            {
                switch (lastThicknessValueSelected)
                {
                    case 1:
                        styleContent.ThicknessBar.ThicknessBarOne.BorderThickness = 0;
                        break;
                    case 2:
                        styleContent.ThicknessBar.ThicknessBarTwo.BorderThickness = 0;
                        break;
                    case 3:
                        styleContent.ThicknessBar.ThicknessBarThird.BorderThickness = 0;
                        break;
                    case 4:
                        styleContent.ThicknessBar.ThicknessBarFourth.BorderThickness = 0;
                        break;
                    case 5:
                        styleContent.ThicknessBar.ThicknessBarFifth.BorderThickness = 0;
                        break;
                }

                switch (thicknessBarIndexSelected)
                {
                    case 1:
                        styleContent.ThicknessBar.ThicknessBarOne.BorderThickness = 1;
                        ChangeThickness(2);
                        break;
                    case 2:
                        styleContent.ThicknessBar.ThicknessBarTwo.BorderThickness = 2;
                        ChangeThickness(4);
                        break;
                    case 3:
                        styleContent.ThicknessBar.ThicknessBarThird.BorderThickness = 2;
                        ChangeThickness(6);
                        break;
                    case 4:
                        styleContent.ThicknessBar.ThicknessBarFourth.BorderThickness = 2;
                        ChangeThickness(8);
                        break;
                    case 5:
                        styleContent.ThicknessBar.ThicknessBarFifth.BorderThickness = 2;
                        ChangeThickness(9);
                        break;
                }
            }

            lastThicknessValueSelected = thicknessBarIndexSelected;
        }

        private void ChangeThickness(int thicknessValue)
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Thickness = thicknessValue;
            else
            {
                if (selectedShapeAnnotation != null)
                    selectedShapeAnnotation.Settings.Thickness = thicknessValue;
                else
                {
                    switch (pdfViewerControl.AnnotationMode)
                    {
                        case AnnotationMode.Ink:
                            pdfViewerControl.AnnotationSettings.Ink.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Rectangle:
                            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Circle:
                            pdfViewerControl.AnnotationSettings.Circle.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Line:
                            pdfViewerControl.AnnotationSettings.Line.Settings.Thickness = thicknessValue;
                            break;
                        case AnnotationMode.Arrow:
                            pdfViewerControl.AnnotationSettings.Arrow.Settings.Thickness = thicknessValue;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void OpacityIcon_Clicked(int opacityValueSelected)
        {
            if (lastOpacityValueSelected != opacityValueSelected)
            {
                switch (lastOpacityValueSelected)
                {
                    case 1:
                        styleContent.OpacityOne.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 2:
                        styleContent.OpacityTwo.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 3:
                        styleContent.OpacityThird.Source = "baseline_invert_colors_off_24.xml";
                        break;
                    case 4:
                        styleContent.OpacityFourth.Source = "baseline_invert_colors_off_24.xml";
                        break;
                }

                switch (opacityValueSelected)
                {
                    case 1:
                        styleContent.OpacityOne.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.25f);
                        break;
                    case 2:
                        styleContent.OpacityTwo.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.50f);
                        break;
                    case 3:
                        styleContent.OpacityThird.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(0.75f);
                        break;
                    case 4:
                        styleContent.OpacityFourth.Source = "baseline_opacity_24.xml";
                        ChangeOpacityValue(1f);
                        break;
                }
            }

            lastOpacityValueSelected = opacityValueSelected;
        }

        private void ChangeOpacityValue(float opacityValue)
        {
            if (selectedInkAnnotation != null)
                selectedInkAnnotation.Settings.Opacity = opacityValue;
            else
            {
                if (selectedShapeAnnotation != null)
                    selectedShapeAnnotation.Settings.Opacity = opacityValue;
                else
                {
                    switch (pdfViewerControl.AnnotationMode)
                    {
                        case AnnotationMode.Ink:
                            pdfViewerControl.AnnotationSettings.Ink.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Rectangle:
                            pdfViewerControl.AnnotationSettings.Rectangle.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Circle:
                            pdfViewerControl.AnnotationSettings.Circle.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Line:
                            pdfViewerControl.AnnotationSettings.Line.Settings.Opacity = opacityValue;
                            break;
                        case AnnotationMode.Arrow:
                            pdfViewerControl.AnnotationSettings.Arrow.Settings.Opacity = opacityValue;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void InitializeSearchErrorPopup()
        {
            searchErrorPopupContent = new SearchErrorPopup();

            errorSearchPopup = new SfPopupLayout
            {
                PopupView = { ShowHeader = false, ShowFooter = false, HeightRequest = 105, WidthRequest = 300 },
                BackgroundColor = Color.FromHex("#fafafa")
            };
            errorSearchPopup.PopupView.BackgroundColor = Color.FromHex("#fafafa");
            errorSearchPopup.PopupView.PopupStyle.BorderColor = Color.FromHex("#fafafa");
            errorSearchPopup.PopupView.AnimationMode = AnimationMode.Fade;

            styleTemplateErrorPopup = new DataTemplate(() => searchErrorPopupContent);

            errorSearchPopup.PopupView.ContentTemplate = styleTemplateErrorPopup;

            if (passwordPopup != null)
                passwordPopup.IsOpen = false;
        }

        private void InitializePopupMenu()
        {
            popupMenu = new SfPopupLayout
            {
                PopupView =
                {
                    IsFullScreen = true,
                    AnimationDuration = 200,
                    AnimationMode = AnimationMode.SlideOnBottom,
                    PopupStyle = {CornerRadius = 0, BorderThickness = 2, BorderColor = Color.White},
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                },
                Padding = new Thickness(15, 15, 15, 15)
            };
            popupMenu.PopupView.ShowFooter = false;
            popupMenu.PopupView.ShowCloseButton = false;
            popupMenu.PopupView.PopupStyle.HeaderBackgroundColor = Color.FromHex("#eeeeee");
            popupMenu.PopupView.PopupStyle.BorderColor = Color.FromHex("#e0e0e0");

            popupMenuContent = new PopupMenuContent();

            var popupMenuContentTemplate = new DataTemplate(() => popupMenuContent);

            var headerTemplateViewPopupMenu = new DataTemplate(() =>
            {
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 0, 13, 0)
                };

                var title = new Label
                {
                    Text = "More",
                    FontSize = 18,
                    FontFamily = "GothamBold.ttf#GothamBold",
                    VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                    HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                    TextColor = Color.FromHex("#4e4e4e"),
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                image = new ImageButton
                {
                    Source = "outlineClearViewer.xml",
                    Aspect = Aspect.AspectFit,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.End
                };

                stackLayout.Children.Add(title);
                stackLayout.Children.Add(image);

                return stackLayout;
            });

            popupMenu.PopupView.HeaderTemplate = headerTemplateViewPopupMenu;

            popupMenu.PopupView.ContentTemplate = popupMenuContentTemplate;
        }

        private void MoreOptionButton_Clicked(object sender, EventArgs e)
        {
            popupMenu.Show();

            popupMenuContent.MenuListView.SelectionChanged += MenuListView_SelectionChanged;
            popupMenu.Closed += PopupMenu_Closed;
            image.Clicked += CloseButtonPopupMenu_Clicked;
        }

        private void PopupMenu_Closed(object sender, EventArgs e)
        {
            popupMenuContent.MenuListView.SelectionChanged -= MenuListView_SelectionChanged;
            popupMenu.Closed -= PopupMenu_Closed;
            image.Clicked -= CloseButtonPopupMenu_Clicked;
        }

        private async void MenuListView_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
        {
            var itemMenu = (ItemsMenu)popupMenuContent.MenuListView.SelectedItem;
            popupMenuContent.MenuListView.SelectedItem = null;

            switch (itemMenu.Id)
            {
                case 0:
                    if (CanSaveDocument)
                        await SaveDocument();
                    break;
                case 1:
                    await PrintDocument();
                    break;
            }
        }

        private async Task SaveDocument()
        {
            Dictionary<bool, string> saveStatus = null;

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.ShowLoading("Loading...", MaskType.Black);
            });

            await Task.Run(async () =>
            {
                using (var stream = await pdfViewerControl.SaveDocumentAsync())
                {
                    if (Device.RuntimePlatform == Device.Android)
                        saveStatus = await DependencyService.Get<IAndroidFileHelper>().SaveAndReturnStatus(stream as MemoryStream, this.filePath);
                }
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.HideLoading();

                if (saveStatus.ContainsKey(true))
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<IToastMessage>().LongAlert("Document saved");

                    popupMenu.IsOpen = false;
                }
                else
                {
                    if (Device.RuntimePlatform == Device.Android)
                        DependencyService.Get<IToastMessage>().LongAlert(saveStatus[false]);
                }
            });

            NumberOfAnnotation = 0;
        }

        private async Task PrintDocument()
        {
            UserDialogs.Instance.ShowLoading("Loading ...", MaskType.Black);

            await Task.Run(() =>
            {
                var fileName = Path.GetFileName(this.filePath);
                pdfViewerControl.Print(fileName);
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.HideLoading();
                popupMenu.IsVisible = false;
            });
        }

        private void CloseButtonPopupMenu_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                popupMenu.IsOpen = false;
            });
        }

        private void PdfViewerControl_PasswordErrorOccurred(object sender, PasswordErrorOccurredEventArgs e)
        {
            InitializePasswordPopup();

            if (HasAlreadyTriedToOpenDoc)
            {
                passwordPopup.Show();
                label.Text = "The password is incorrect. Please try again";
                entry.Focus();
            }
            else
            {
                passwordPopup.Show();
                HasAlreadyTriedToOpenDoc = true;
            }
        }

        private void InitializePasswordPopup()
        {
            if (IsPasswordPopupInitialized == false)
            {
                passwordPopup = new SfPopupLayout
                {
                    PopupView =
                    {
                        HeightRequest = 200,
                        WidthRequest = 310,
                        ShowHeader = true,
                        ShowFooter = true,
                        ShowCloseButton = false,
                        AnimationDuration = 170,
                        HeaderHeight = 63
                    }
                };
                passwordPopup.Closed += PasswordPopup_Closed;
                passwordPopup.StaysOpen = true;

                var templateViewHeaderSetPasswordPoop = new DataTemplate(() =>
                {
                    var label = new Label
                    {
                        VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                        Padding = new Thickness(20, 0, 20, 0),
                        Text = "Password",
                        TextColor = Color.Black,
                        FontSize = 17,
                        FontFamily = "GothamMedium_1.ttf#GothamMedium_1"
                    };

                    return label;
                });


                templateViewSetPasswordPopup = new DataTemplate(() =>
                {
                    var stackLayout = new StackLayout();

                    label = new Label
                    {
                        Text = "This file is password protected. Please enter the password",
                        TextColor = Color.Black,
                        FontSize = 13.5,
                        Padding = new Thickness(20, 0, 20, 0),
                        FontFamily = "GothamMedium_1.ttf#GothamMedium_1"
                    };


                    entry = new Entry
                    {
                        FontSize = 13.5,
                        Margin = new Thickness(19, 0, 19, 0),
                        IsPassword = true,
                        Placeholder = "Enter the password",
                        TextColor = Color.Black
                    };

                    stackLayout.Children.Add(label);
                    stackLayout.Children.Add(entry);

                    return stackLayout;
                });

                var footerTemplateViewSetPasswordPopup = new DataTemplate(() =>
                {
                    var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0 };

                    acceptButton = new Button
                    {
                        Text = "Ok",
                        FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                        FontSize = 14,
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        BackgroundColor = Color.White,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 86
                    };
                    acceptButton.Clicked += AcceptButtonPasswordPopup_Clicked;
                    declineButton = new Button
                    {
                        Text = "Cancel",
                        FontFamily = "GothamMedium_1.ttf#GothamMedium_1",
                        FontSize = 14,
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.End,
                        BackgroundColor = Color.White,
                        VerticalOptions = LayoutOptions.Center,
                        WidthRequest = 89
                    };
                    declineButton.Clicked += DeclineButton_Clicked;

                    stackLayout.Children.Add(acceptButton);
                    stackLayout.Children.Add(declineButton);

                    return stackLayout;
                });

                // Adding ContentTemplate of the SfPopupLayout
                passwordPopup.PopupView.ContentTemplate = templateViewSetPasswordPopup;

                // Adding FooterTemplate of the SfPopupLayout
                passwordPopup.PopupView.FooterTemplate = footerTemplateViewSetPasswordPopup;

                // Adding FooterTemplate of the SfPopupLayout
                passwordPopup.PopupView.HeaderTemplate = templateViewHeaderSetPasswordPoop;

                IsPasswordPopupInitialized = true;

                Shell.SetNavBarIsVisible(this, false);
                Shell.SetTabBarIsVisible(this, false);
                Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
                NavigationPage.SetHasNavigationBar(this, false);
            }
        }

        private void PasswordPopup_Closed(object sender, EventArgs e)
        {
            if (IsATryToOpenDocument)
            {
                pdfViewerControl.LoadDocument(pdfStream, entry.Text);
                IsATryToOpenDocument = false;
            }
        }

        private void AcceptButtonPasswordPopup_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading("Loading", MaskType.Black);

            passwordPopup.IsOpen = false;
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();

            IsATryToOpenDocument = true;
        }

        private async void DeclineButton_Clicked(object sender, EventArgs e)
        {
            passwordPopup.IsOpen = false;
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();

            await Navigation.PopAsync();
        }
    }
}
