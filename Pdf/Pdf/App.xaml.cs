﻿using Pdf.Api;
using Pdf.Data;
using Pdf.Views;
using System;
using System.ComponentModel;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Pdf
{
    public partial class App : Application
    {
        public static UnityContainer Container { get; set; }

        public App()
        {
            InitializeComponent();

            Device.SetFlags(new[] {
                "CarouselView_Experimental",
                "IndicatorView_Experimental"
            });

            Container = new UnityContainer();
            Container.RegisterType<IConcateEndpoint, ConcateEndpoint>();
            Container.RegisterType<IDeconcateEndpoint, DeconcateEndpoint>();
            Container.RegisterType<IGetFilesEndpoint, GetFilesEndpoint>();
            Container.RegisterType<IOverlayEndpoint, OverlayEndpoint>();
            Container.RegisterType<IPasswordEndpoint, PasswordEndpoint>();
            Container.RegisterType<IRemovePagesEndpoint, RemovePagesEndpoint>();
            Container.RegisterType<ISummaryEndpoint, SummaryEndpoint>();
            Container.RegisterType<IUncompressEndpoint, UncompressEndpoint>();

            Container.RegisterType<IDocumentsData, DocumentsData>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<IDocumentsData, DocumentsData>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new object[] { new DocumentsData()}));

            MainPage = /*new NavigationPage(new MainMenu());*/
                new ShellMenu();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
