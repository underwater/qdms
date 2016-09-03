﻿// -----------------------------------------------------------------------
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

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for AddInstrumentQuandlWindow.xaml
    /// </summary>
    public partial class AddInstrumentFredWindow : MetroWindow
    {
        public ObservableCollection<FredUtils.FredSeries> Series { get; set; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public List<Instrument> AddedInstruments { get; set; }

        private readonly Datasource _thisDS;

        private const string ApiKey = "f8d71bdcf1d7153e157e0baef35f67db";

        public AddInstrumentFredWindow(MyDBContext context)
        {
            DataContext = this;

            AddedInstruments = new List<Instrument>();

            Series = new ObservableCollection<FredUtils.FredSeries>();

            InitializeComponent();

            _thisDS = context.Datasources.First(x => x.Name == "FRED");

            ShowDialog();
        }

        private async void Search()
        {
            Series.Clear();
            StatusLabel.Content = "Searching...";

            var foundSeries = await FredUtils.FindSeries(SymbolTextBox.Text, ApiKey);
            foreach (var i in foundSeries)
            {
                Series.Add(i);
            }

            StatusLabel.Content = foundSeries.Count() + " contracts found";
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
            int count = 0;

            var instrumentSource = new InstrumentManager();

            foreach (FredUtils.FredSeries series in InstrumentGrid.SelectedItems)
            {
                var newInstrument = FredUtils.SeriesToInstrument(series);
                newInstrument.Datasource = _thisDS;

                try
                {
                    if (instrumentSource.AddInstrument(newInstrument) != null)
                        count++;
                    AddedInstruments.Add(newInstrument);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            StatusLabel.Content = string.Format("{0}/{1} instruments added.", count, InstrumentGrid.SelectedItems.Count);
        }
    }
}
