// -----------------------------------------------------------------------
// <copyright file="AddInstrumentManuallyWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EntityData;
using QDMS;
using QDMSServer.Helpers;
using QDMSServer.ViewModels;
using ReactiveUI;

namespace QDMSServer
{
    public partial class AddInstrumentManuallyWindow
    {
        public Instrument Instrument { get; set; }

        public bool InstrumentAdded = false;

        public ManualViewModel ViewModel { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instrument">If we're updating or cloning an instrument, pass it here.</param>
        /// <param name="addingNew">True if adding a new instrument. False if we're updating an instrument.</param>
        /// <param name="addingContFut">True if adding a continuous futures instrument.</param>
        public AddInstrumentManuallyWindow(bool addingNew, bool addingContFut = false)
        {
            InitializeComponent();

            if (addingNew)
                ViewModel = new ManualViewModel(false);
            else
                ViewModel = new ManualViewModel();

            //If it's a continuous future, make the continuous future tab visible
            if ((ViewModel.Instrument != null && ViewModel.Instrument.IsContinuousFuture) ||
                addingContFut)
            {
                ContFutTabItem.Visibility = Visibility.Visible;
                TypeComboBox.IsEnabled = false;
            }
            else
            {
                ContFutTabItem.Visibility = Visibility.Hidden;
            }

            DataContext = ViewModel;


            //Window title
            if (addingNew)
            {
                Title = "Add New Instrument";
                AddBtn.Content = "Add";
            }
            else
            {
                Title = "Modify Instrument";
                AddBtn.Content = "Modify";
            }


            //sort the sessions so they're ordered properly...
            SessionsGrid.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("OpeningDay", System.ComponentModel.ListSortDirection.Ascending));

            MessageBus.Current.Listen<string>().Subscribe(message =>
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ViewModel.ErrorMessage += "*" + message + "\r\n";
            });

            MessageBus.Current.Listen<ShowErrorMessage>().Subscribe(x =>
            {
                if (x == ShowErrorMessage.Visible)
                {
                    ErrorMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorMessage.Visibility = Visibility.Collapsed;
                    ViewModel.ErrorMessage = "";
                }
            });

            ViewModel.CloseCommand.Subscribe(_ =>
            {
                Hide();
            });

        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Dispose();
        }

    }
}