// -----------------------------------------------------------------------
// <copyright file="ExchangesWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EntityData;
using MahApps.Metro.Controls;
using ReactiveUI;
using QDMSServer.ViewModels.Exchanges;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for ExchangesWindow.xaml
    /// </summary>
    public partial class ExchangesWindow : MetroWindow, IViewFor<ExchangesViewModel>
    {
        public ExchangesViewModel ViewModel { get; set; }

        object IViewFor.ViewModel { get; set; }

        public ExchangesWindow()
        {
            InitializeComponent();
            ViewModel = new ExchangesViewModel();
            this.WhenAnyObservable(x => x.ViewModel.AddCommand)
                .Subscribe(_ =>
                {
                    new EditExchangeWindow(ViewModel).ShowDialog();
                });

            this.WhenAnyObservable(x => x.ViewModel.ModifyCommand)
                .Subscribe(_ =>
                {
                    new EditExchangeWindow(ViewModel, true).ShowDialog();
                });

            ViewModel.DeleteCommand.Subscribe(_ =>
            {
                using (var context = new QDMSDbContext())
                {
                    var instrumentCount = context.Instruments.Count(x => x.ExchangeID == ViewModel.SelectedExchange.ID);
                    if (instrumentCount > 0)
                    {
                        MessageBox.Show(string.Format("Can't delete this exchange it has {0} instruments assigned to it.", instrumentCount));
                        return;
                    }
                }

                var result = MessageBox.Show(
                      string.Format("Are you sure you want to delete {0}?"
                    , ViewModel.SelectedExchange.Name)
                    , "Delete"
                    , MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                    ViewModel.ConfirmDeleteCommand.Execute(null);
            });

            DataContext = ViewModel;
        }

        private void TableView_RowDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (ViewModel.SelectedExchange != null)
            {
                new EditExchangeWindow(ViewModel, true).ShowDialog();
            }
        }     
    }
}
