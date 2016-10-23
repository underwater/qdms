// -----------------------------------------------------------------------
// <copyright file="SessionTemplatesWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using EntityData;
using MahApps.Metro.Controls;
using QDMS;
using QDMSServer.ViewModels.RootSymbol;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for SessionTemplatesWindow.xaml
    /// </summary>
    public partial class RootSymbolsWindow : MetroWindow
    {
        public RootSymbolsViewModel ViewModel { get; set; }

        public ObservableCollection<UnderlyingSymbol> Symbols { get; set; }

        public RootSymbolsWindow()
        {
            InitializeComponent();
            ViewModel = new RootSymbolsViewModel();
            DataContext = ViewModel;
            ViewModel.AddCommand.Subscribe(_ =>
            {
                var window = new EditRootSymbolWindow(ViewModel);
                window.ShowDialog();
            });

            ViewModel.ModifyCommand.Subscribe(_ =>
            {
                var window = new EditRootSymbolWindow(ViewModel, true);
                window.ShowDialog();
            });

            ViewModel.DeleteCommand.Subscribe(_ =>
            {
                var result = MessageBox.Show(string.Format("Are you sure you want to delete {0}?", ViewModel.SelectedSymbol.Symbol), "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    ViewModel.ConfirmDeleteCommand.Execute(null);
            });
                        
        }

        private void TableView_RowDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            //var window = new EditRootSymbolWindow((UnderlyingSymbol)SymbolsGrid.SelectedItem);
            //window.ShowDialog();
            //CollectionViewSource.GetDefaultView(SymbolsGrid.ItemsSource).Refresh();
        }
    }
}
