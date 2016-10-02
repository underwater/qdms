// -----------------------------------------------------------------------
// <copyright file="SessionTemplatesWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using QDMSServer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for SessionTemplatesWindow.xaml
    /// </summary>
    public partial class SessionTemplatesWindow : MetroWindow, IViewFor<SessionTemplatesViewModel>
    {
        public SessionTemplatesViewModel ViewModel { get; set; }

        object IViewFor.ViewModel { get; set; }

        public SessionTemplatesWindow()
        {
            InitializeComponent();
            ViewModel = new SessionTemplatesViewModel();

            ViewModel.AddCommand.Subscribe(_ =>
            {
                new EditSessionTemplateWindow(ViewModel).ShowDialog();
            });

            ViewModel.ModifyCommand.Subscribe(_ =>
            {
                new EditSessionTemplateWindow(ViewModel, true).ShowDialog();
            });

            ViewModel.DeleteCommand.Subscribe(_ =>
            {
                var result = MessageBox.Show($"Are you sure you want to delete {ViewModel.SelectedTemplate.Name}?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    ViewModel.ConfirmDeleteCommand.Execute(null);
            });
            DataContext = ViewModel;
        }

        private void TableView_RowDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (ViewModel.SelectedTemplate != null)
            {
                new EditSessionTemplateWindow(ViewModel, true).ShowDialog();
            }
        }
    }
}
