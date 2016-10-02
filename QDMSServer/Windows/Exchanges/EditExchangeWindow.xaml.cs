// -----------------------------------------------------------------------
// <copyright file="EditExchangeWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using EntityData;
using MahApps.Metro.Controls;
using QDMS;
using QDMSServer.ViewModels.Exchanges;
using ReactiveUI;
using System.Windows.Data;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for EditExchangeWindow.xaml
    /// </summary>
    public partial class EditExchangeWindow : MetroWindow
    {
        public EditExchangeViewModel ViewModel { get; set; }

        public EditExchangeWindow(ExchangesViewModel exchangesViewModel, bool isModify = false)
        {
            InitializeComponent();
            if (!isModify)
            {
                exchangesViewModel.SelectedExchange = null;
            }
            ViewModel = new EditExchangeViewModel(exchangesViewModel);

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

            DataContext = ViewModel;
        }
    }
}
