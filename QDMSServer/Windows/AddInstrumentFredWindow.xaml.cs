// -----------------------------------------------------------------------
// <copyright file="AddInstrumentFredlWindow.xaml.cs" company="">
// Copyright 2014 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EntityData;
using MahApps.Metro.Controls;
using NLog;
using QDMS;
using ReactiveUI;
using QDMSServer.ViewModels;
using System.Reactive.Linq;
using System.Windows.Controls;
using Splat;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for AddInstrumentQuandlWindow.xaml
    /// </summary>
    public partial class AddInstrumentFredWindow : MetroWindow, IViewFor<FredViewModel>, IActivatable
    {
        public FredViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }

            set { ViewModel = (FredViewModel)value; }
        }


        public AddInstrumentFredWindow()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<FredViewModel>();
            DataContext = ViewModel;

            this.WhenActivated(d => 
            {

            });

            IDisposable obj =  Observable.FromEventPattern<SelectionChangedEventArgs>(InstrumentGrid, nameof(InstrumentGrid.SelectionChanged))
                .Subscribe(x => ViewModel.SelectedItems = x.EventArgs.AddedItems.Cast<FredUtils.FredSeries>().ToList());
                    
            Observable.FromEventPattern<KeyEventArgs>(SearchTextBox, nameof(SearchTextBox.KeyDown))
                .Where(e => e.EventArgs.Key == Key.Enter)
                .InvokeCommand(ViewModel.SearchCommand);

            SearchTextBox.KeyDown += SearchTextBox_KeyDown;
            this.WhenAnyObservable(x => x.ViewModel.CloseCommand)
                .Subscribe(x => Hide());            
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ViewModel.SearchCommand.Execute(null);
            }
        }
    }
}
