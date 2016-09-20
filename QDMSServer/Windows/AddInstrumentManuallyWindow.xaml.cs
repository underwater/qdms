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

        public ObservableCollection<CheckBoxTag> Tags { get; set; }

        public ObservableCollection<Exchange> Exchanges { get; set; }

        public ObservableCollection<InstrumentSession> SelectedSessions { get; set; } //todo implement
        

        public bool InstrumentAdded = false;

        private readonly bool _addingNew;
        private readonly QDMSDbContext _context;

        /// <summary>
        ///
        /// </summary>
        /// <param name="instrument">If we're updating or cloning an instrument, pass it here.</param>
        /// <param name="addingNew">True if adding a new instrument. False if we're updating an instrument.</param>
        /// <param name="addingContFut">True if adding a continuous futures instrument.</param>
        public AddInstrumentManuallyWindow(Instrument instrument = null, bool addingNew = true, bool addingContFut = false)
        {
            InitializeComponent();

            var viewModel = new ManualViewModel(false);

            //If it's a continuous future, make the continuous future tab visible
            if ((instrument != null && instrument.IsContinuousFuture) ||
                addingContFut)
            {
                ContFutTabItem.Visibility = Visibility.Visible;
                TypeComboBox.IsEnabled = false;
            }
            else
            {
                ContFutTabItem.Visibility = Visibility.Hidden;
            }

            DataContext = viewModel;
            _addingNew = addingNew;

            _context = new QDMSDbContext();




            //CustomRadioBtn.IsChecked = true;


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
                viewModel.ErrorMessage = "*" + message;
            });

            MessageBus.Current.Listen<ShowErrorMessage>().Subscribe(x =>
            {
                if (x == ShowErrorMessage.Visible)
                    ErrorMessage.Visibility = Visibility.Visible;
                else
                    ErrorMessage.Visibility = Visibility.Collapsed;
            });

            viewModel.CloseCommand.Subscribe(_ => 
            {
                Hide();
            });
            
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            //if (_addingNew &&
            //    _context.Instruments.Any(
            //        x => x.DatasourceID == Instrument.DatasourceID &&
            //                x.ExchangeID == Instrument.ExchangeID &&
            //                x.Symbol == Instrument.Symbol &&
            //                x.Expiration == Instrument.Expiration)
            //    )
            //{
            //    //there's already an instrument with this key
            //    MessageBox.Show("Instrument already exists. Change datasource, exchange, or symbol.");
            //    return;
            //}

            ////check that if the user picked a template-based session set, he actually selected one of the templates
            //if (Instrument.SessionsSource == SessionsSource.Template && Instrument.SessionTemplateID == -1)
            //{
            //    MessageBox.Show("You must pick a session template.");
            //    return;
            //}

            //if (Instrument.IsContinuousFuture && Instrument.Type != InstrumentType.Future)
            //{
            //    MessageBox.Show("Continuous futures type must be Future.");
            //    return;
            //}

            //if (Instrument.Datasource == null)
            //{
            //    MessageBox.Show("You must select a data source.");
            //    return;
            //}

            //if (Instrument.Multiplier == null)
            //{
            //    MessageBox.Show("Must have a multiplier value.");
            //    return;
            //}

            ////Validate the sessions
            //if (SelectedSessions.Count > 0)
            //{
            //    try
            //    {
            //        MyUtils.ValidateSessions(SelectedSessions.ToList<ISession>());
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //        return;
            //    }
            //}

            ////move selected sessions to the instrument
            //Instrument.Sessions.Clear();
            //foreach (InstrumentSession s in SelectedSessions)
            //{
            //    //need to attach?
            //    Instrument.Sessions.Add(s);
            //}
            
            //Instrument.Tags.Clear();

            //foreach (Tag t in Tags.Where(x => x.IsChecked).Select(x => x.Item))
            //{
            //    _context.Tags.Attach(t);
            //    Instrument.Tags.Add(t);
            //}

            //ContinuousFuture tmpCF = null;

            //if (_addingNew)
            //{
            //    if (Instrument.Exchange != null) _context.Exchanges.Attach(Instrument.Exchange);
            //    if (Instrument.PrimaryExchange != null) _context.Exchanges.Attach(Instrument.PrimaryExchange);
            //    _context.Datasources.Attach(Instrument.Datasource);

            //    if (Instrument.IsContinuousFuture)
            //    {
            //        tmpCF = Instrument.ContinuousFuture; //EF can't handle circular references, so we hack around it
            //        Instrument.ContinuousFuture = null;
            //        Instrument.ContinuousFutureID = null;
            //    }
            //    _context.Instruments.Add(Instrument);
            //}
            //else //simply manipulating an existing instrument
            //{
            //    //make sure any "loose" sessions are deleted
            //    if (!_addingNew)
            //    {
            //        foreach (InstrumentSession s in _originalSessions.Where(s => !Instrument.Sessions.Any(x => x.ID == s.ID)))
            //        {
            //            _context.InstrumentSessions.Remove(s);
            //        }
            //    }
            //}

            //_context.Database.Connection.Open();
            //_context.SaveChanges();

            //if (tmpCF != null)
            //{
            //    _context.UnderlyingSymbols.Attach(tmpCF.UnderlyingSymbol);

            //    Instrument.ContinuousFuture = tmpCF;
            //    Instrument.ContinuousFuture.Instrument = Instrument;
            //    Instrument.ContinuousFuture.InstrumentID = Instrument.ID.Value;
            //    _context.SaveChanges();
            //}

            //InstrumentAdded = true;
            //Hide();
        }


        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OptionTypeNullCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkEdit = sender as CheckBox;
            if (checkEdit != null && (!checkEdit.IsChecked.HasValue || checkEdit.IsChecked.Value == false))
            {
                Instrument.OptionType = (OptionType?)OptionTypeComboBox.SelectedItem;
            }
            else
            {
                Instrument.OptionType = null;
            }
        }

        private void ExchangeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (Instrument.Exchange == null) return;
            if (Instrument.SessionsSource == SessionsSource.Exchange) return; //we don't want to re-load them if it's already set

            SelectedSessions.Clear();
            Instrument.SessionsSource = SessionsSource.Exchange;

            foreach (ExchangeSession s in Instrument.Exchange.Sessions)
            {
                SelectedSessions.Add(s.ToInstrumentSession());
            }
        }

        private void CustomRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            Instrument.SessionsSource = SessionsSource.Custom;
        }

        private void TemplateRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            //Instrument.SessionsSource = SessionsSource.Template;
            //FillSessionsFromTemplate();
        }

        private void TemplateComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            //FillSessionsFromTemplate();
        }

        private void FillSessionsFromTemplate()
        {
            SelectedSessions.Clear();

            var template = (SessionTemplate)TemplateComboBox.SelectedItem;
            if (template == null)
            {
                Instrument.SessionTemplateID = -1; //we can check for this later and deny the new instrument if its sessions are not set properly
                return;
            }

            Instrument.SessionTemplateID = template.ID;
            foreach (TemplateSession s in template.Sessions.OrderBy(x => x.OpeningDay))
            {
                SelectedSessions.Add(s.ToInstrumentSession());
            }
        }

        private void AddSessionItemBtn_Click(object sender, RoutedEventArgs e)
        {
            var toAdd = new InstrumentSession { IsSessionEnd = true };

            if (SelectedSessions.Count == 0)
            {
                toAdd.OpeningDay = DayOfTheWeek.Monday;
                toAdd.ClosingDay = DayOfTheWeek.Monday;
            }
            else
            {
                DayOfTheWeek maxDay = (DayOfTheWeek)Math.Min(6, SelectedSessions.Max(x => (int)x.OpeningDay) + 1);
                toAdd.OpeningDay = maxDay;
                toAdd.ClosingDay = maxDay;
            }
            SelectedSessions.Add(toAdd);
        }

        private void DeleteSessionItemBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedSession = (InstrumentSession)SessionsGrid.SelectedItem;
            SelectedSessions.Remove(selectedSession);
        }

        private void ExchangeComboBox_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (Instrument.SessionsSource == SessionsSource.Exchange)
            {
                SelectedSessions.Clear();

                foreach (ExchangeSession s in Instrument.Exchange.Sessions)
                {
                    SelectedSessions.Add(s.ToInstrumentSession());
                }
            }
        }


        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
        
    }
}