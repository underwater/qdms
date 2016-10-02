// -----------------------------------------------------------------------
// <copyright file="EditSessionTemplateWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EntityData;
using MahApps.Metro.Controls;
using QDMS;
using QDMSServer.ViewModels.SessionTemplate;
using QDMSServer.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Data;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for EditSessionTemplateWindow.xaml
    /// </summary>
    public partial class EditSessionTemplateWindow : MetroWindow
    {
        public EditSessionTemplateViewModel ViewModel { get; set; }

        public EditSessionTemplateWindow(SessionTemplatesViewModel sessionTemplatesViewModel = null, bool isModify = false)
        {
            InitializeComponent();

            if (!isModify)
            {
                sessionTemplatesViewModel.SelectedTemplate = null;
            }

            ViewModel = new EditSessionTemplateViewModel(sessionTemplatesViewModel);

            this.DataContext = ViewModel;

            this.WhenAnyObservable(x => x.ViewModel.AddSessionCommand)
                .Subscribe(_ =>
                {
                    CollectionViewSource.GetDefaultView(SessionsGrid.ItemsSource).Refresh();
                });
            this.WhenAnyObservable(x => x.ViewModel.DeleteSessionCommand)
                .Subscribe(_ =>
                {
                    CollectionViewSource.GetDefaultView(SessionsGrid.ItemsSource).Refresh();
                });

            this.WhenAnyObservable(x => x.ViewModel.CloseCommand).Subscribe(x => Hide());

            MessageBus.Current.Listen<Exception>()
                .Subscribe(ex =>
                {
                    MessageBox.Show(ex.Message);
                });
        }
    }
}
