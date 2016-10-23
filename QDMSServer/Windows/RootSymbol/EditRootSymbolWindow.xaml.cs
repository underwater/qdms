// -----------------------------------------------------------------------
// <copyright file="EditRootSymbolWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using EntityData;
using MahApps.Metro.Controls;
using QDMS;
using QDMSServer.ViewModels.RootSymbol;
using ReactiveUI;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for EditRootSymbolWindow.xaml
    /// </summary>
    public partial class EditRootSymbolWindow : MetroWindow
    {
        public EditRootSymbolsViewModel ViewModel { get; set; }
        
        public EditRootSymbolWindow(RootSymbolsViewModel viewModel, bool isEdit = false)
        {
            InitializeComponent();
            if (!isEdit)
            {
                viewModel.SelectedSymbol = null;
            }
            ViewModel = new EditRootSymbolsViewModel(viewModel);
            DataContext = ViewModel;
            this.WhenAnyObservable(x => x.ViewModel.CloseCommand)
                .Subscribe(_ =>
                {
                    Hide();
                    ViewModel.Dispose();
                });
            this.WhenAnyObservable(x => x.ViewModel.SaveCommand.ThrownExceptions)
                .Subscribe(ex =>
                {
                    MessageBox.Show(ex.Message);
                });

        }
    }
}
