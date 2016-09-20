// -----------------------------------------------------------------------
// <copyright file="AddInstrumentQuandlWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
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
using Splat;
using QDMSServer.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for AddInstrumentQuandlWindow.xaml
    /// </summary>
    public partial class AddInstrumentQuandlWindow : MetroWindow, IViewFor<QuandlViewModel>
    {
        public QuandlViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get
            {
                return ViewModel;
            }

            set
            {
                ViewModel = (QuandlViewModel)value;
            }
        }

        public AddInstrumentQuandlWindow()
        {
            InitializeComponent();
            ViewModel = Locator.Current.GetService<QuandlViewModel>();
            DataContext = ViewModel;

            Observable.FromEventPattern<KeyEventArgs>(SearchTextBox, nameof(SearchTextBox.KeyDown))
                .Where(e => e.EventArgs.Key == Key.Enter)
                .InvokeCommand(ViewModel.SearchCommand);

            Observable.FromEventPattern<SelectionChangedEventArgs>(InstrumentGrid, nameof(InstrumentGrid.SelectionChanged))
                
               .Subscribe(x =>
               {
                   ViewModel.SelectedItems = InstrumentGrid.SelectedItems.Cast<Instrument>().ToList();
               });

            this.WhenAnyObservable(x => x.ViewModel.CloseCommand)
                .Subscribe(x => Hide());

            ShowDialog();
        }

        private void Search(int page = 1)
        {
            //Instruments.Clear();
            //int count = 0;
            ////var foundInstruments = QuandlUtils.FindInstruments(SymbolTextBox.Text, out count, page);
            ////foreach (var i in foundInstruments)
            ////{
            ////    i.Datasource = _thisDS;
            ////    i.DatasourceID = _thisDS.ID;
            ////    i.Multiplier = 1;
            ////    Instruments.Add(i);
            ////}

            //StatusLabel.Content = count + " contracts found";

            //CurrentPageTextBox.Text = page.ToString();
        }

        private void SymbolTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            //int count = 0;
            //var instrumentSource = new InstrumentManager();

            //foreach (Instrument newInstrument in InstrumentGrid.SelectedItems)
            //{
            //    if (newInstrument.Exchange != null)
            //        newInstrument.ExchangeID = newInstrument.Exchange.ID;
            //    if (newInstrument.PrimaryExchange != null)
            //        newInstrument.PrimaryExchangeID = newInstrument.PrimaryExchange.ID;

            //    try
            //    {
            //        if (instrumentSource.AddInstrument(newInstrument) != null)
            //            count++;
            //        AddedInstruments.Add(newInstrument);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "Error");
            //    }
            //}
            //StatusLabel.Content = string.Format("{0}/{1} instruments added.", count, InstrumentGrid.SelectedItems.Count);
        }

        private void PageForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            //int currentPage;
            //bool parsed = int.TryParse(CurrentPageTextBox.Text, out currentPage);
            //if (parsed)
            //{
            //    currentPage++;
            //    Search(currentPage);
            //}
        }

        private void PageBackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //int currentPage;
            //bool parsed = int.TryParse(CurrentPageTextBox.Text, out currentPage);
            //if (parsed && currentPage > 1)
            //{
            //    currentPage++;
            //    Search(currentPage);
            //}
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    int currentPage;
            //    bool parsed = int.TryParse(CurrentPageTextBox.Text, out currentPage);
            //    if (parsed)
            //    {
            //        currentPage++;
            //        Search(currentPage);
            //    }
            //}
        }
    }
}
