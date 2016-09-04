﻿// -----------------------------------------------------------------------
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

namespace QDMSServer
{
    public partial class AddInstrumentManuallyWindow
    {
        public Instrument TheInstrument { get; set; }

        public ObservableCollection<CheckBoxTag> Tags { get; set; }

        public ObservableCollection<Exchange> Exchanges { get; set; }

        public ObservableCollection<InstrumentSession> SelectedSessions { get; set; } //todo implement

        public ObservableCollection<KeyValuePair<int, string>> ContractMonths { get; set; }

        public bool InstrumentAdded = false;

        private readonly List<InstrumentSession> _originalSessions;
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

            DataContext = this;
            _addingNew = addingNew;

            _context = new QDMSDbContext();

            if (instrument != null)
            {
                _context.Instruments.Attach(instrument);
                _context.Entry(instrument).Reload();
                if (instrument.Exchange != null)
                    _context.Entry(instrument.Exchange).Reload();

                if (instrument.ContinuousFuture != null)
                {
                    _context.ContinuousFutures.Attach(instrument.ContinuousFuture);
                    _context.Entry(instrument.ContinuousFuture).Reload();
                }

                if (!addingNew)
                {
                    if (instrument.Tags != null)
                    {
                        foreach (Tag tag in instrument.Tags)
                        {
                            _context.Tags.Attach(tag);
                        }
                    }

                    if (instrument.Sessions != null)
                    {
                        foreach (InstrumentSession session in instrument.Sessions)
                        {
                            _context.InstrumentSessions.Attach(session);
                        }
                    }
                }

                TheInstrument = addingNew ? (Instrument)instrument.Clone() : instrument;
                if (TheInstrument.Tags == null) TheInstrument.Tags = new List<Tag>();
                if (TheInstrument.Sessions == null) TheInstrument.Sessions = new List<InstrumentSession>();

                TheInstrument.Sessions = TheInstrument.Sessions.OrderBy(x => x.OpeningDay).ThenBy(x => x.OpeningTime).ToList();

                _originalSessions = new List<InstrumentSession>(TheInstrument.Sessions);
            }
            else
            {
                TheInstrument = new Instrument
                {
                    Tags = new List<Tag>(),
                    Sessions = new List<InstrumentSession>()
                };

                //need to do some extra stuff if it's a continuous future
                if (addingContFut)
                {
                    TheInstrument.ContinuousFuture = new ContinuousFuture();
                    TheInstrument.Type = InstrumentType.Future;
                    TheInstrument.IsContinuousFuture = true;
                }

                CustomRadioBtn.IsChecked = true;
            }

            //Tags
            Tags = new ObservableCollection<CheckBoxTag>();
            foreach (Tag t in _context.Tags)
            {
                Tags.Add(new CheckBoxTag(t, TheInstrument.Tags.Contains(t)));
            }

            //Sessions
            SelectedSessions = new ObservableCollection<InstrumentSession>(TheInstrument.Sessions);

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

            Exchanges = new ObservableCollection<Exchange>();

            var exchangeList = _context.Exchanges.AsEnumerable().OrderBy(x => x.Name);
            foreach (Exchange e in exchangeList)
            {
                Exchanges.Add(e);
            }

            //fill template box
            var templates = _context.SessionTemplates.Include("Sessions").ToList();
            foreach (SessionTemplate t in templates)
            {
                TemplateComboBox.Items.Add(t);
            }
            if (TheInstrument.SessionsSource == SessionsSource.Template)
            {
                TemplateComboBox.SelectedItem = templates.First(x => x.ID == TheInstrument.SessionTemplateID);
            }

            //set the right radio button...
            CustomRadioBtn.IsChecked = TheInstrument.SessionsSource == SessionsSource.Custom;
            TemplateRadioBtn.IsChecked = TheInstrument.SessionsSource == SessionsSource.Template;
            ExchangeRadioBtn.IsChecked = TheInstrument.SessionsSource == SessionsSource.Exchange;

            //populate instrument type combobox with enum values
            var instrumentTypeValues = MyUtils.GetEnumValues<InstrumentType>();
            foreach (InstrumentType t in instrumentTypeValues)
            {
                TypeComboBox.Items.Add(t);
            }

            //populate option type combobox with enum values
            var optionTypeValues = MyUtils.GetEnumValues<OptionType>();
            foreach (OptionType t in optionTypeValues)
            {
                OptionTypeComboBox.Items.Add(t);
            }

            var dataSources = _context.Datasources.AsEnumerable();
            foreach (Datasource d in dataSources)
            {
                DatasourceComboBox.Items.Add(d);
            }

            //sort the sessions so they're ordered properly...
            SessionsGrid.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("OpeningDay", System.ComponentModel.ListSortDirection.Ascending));

            //fill the RolloverRuleType combobox
            var rolloverTypes = MyUtils.GetEnumValues<ContinuousFuturesRolloverType>();
            foreach (ContinuousFuturesRolloverType t in rolloverTypes)
            {
                if (t != ContinuousFuturesRolloverType.Time)
                    RolloverRuleType.Items.Add(t);
            }

            //fill the RootSymbolComboBox
            foreach (UnderlyingSymbol s in _context.UnderlyingSymbols)
            {
                RootSymbolComboBox.Items.Add(s);
            }

            ContractMonths = new ObservableCollection<KeyValuePair<int, string>>();
            //fill the continuous futures contrat month combobox
            for (int i = 1; i < 10; i++)
            {
                ContractMonths.Add(new KeyValuePair<int, string>(i, MyUtils.Ordinal(i) + " Contract"));
            }

            //time or rule-based rollover, set the radio button check
            if (TheInstrument.ContinuousFuture != null)
            {
                if (TheInstrument.ContinuousFuture.RolloverType == ContinuousFuturesRolloverType.Time)
                {
                    RolloverTime.IsChecked = true;
                }
                else
                {
                    RolloverRule.IsChecked = true;
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_addingNew &&
                _context.Instruments.Any(
                    x => x.DatasourceID == TheInstrument.DatasourceID &&
                            x.ExchangeID == TheInstrument.ExchangeID &&
                            x.Symbol == TheInstrument.Symbol &&
                            x.Expiration == TheInstrument.Expiration)
                )
            {
                //there's already an instrument with this key
                MessageBox.Show("Instrument already exists. Change datasource, exchange, or symbol.");
                return;
            }

            //check that if the user picked a template-based session set, he actually selected one of the templates
            if (TheInstrument.SessionsSource == SessionsSource.Template && TheInstrument.SessionTemplateID == -1)
            {
                MessageBox.Show("You must pick a session template.");
                return;
            }

            if (TheInstrument.IsContinuousFuture && TheInstrument.Type != InstrumentType.Future)
            {
                MessageBox.Show("Continuous futures type must be Future.");
                return;
            }

            if (TheInstrument.Datasource == null)
            {
                MessageBox.Show("You must select a data source.");
                return;
            }

            if (TheInstrument.Multiplier == null)
            {
                MessageBox.Show("Must have a multiplier value.");
                return;
            }

            //Validate the sessions
            if (SelectedSessions.Count > 0)
            {
                try
                {
                    MyUtils.ValidateSessions(SelectedSessions.ToList<ISession>());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            //move selected sessions to the instrument
            TheInstrument.Sessions.Clear();
            foreach (InstrumentSession s in SelectedSessions)
            {
                //need to attach?
                TheInstrument.Sessions.Add(s);
            }
            
            TheInstrument.Tags.Clear();

            foreach (Tag t in Tags.Where(x => x.IsChecked).Select(x => x.Item))
            {
                _context.Tags.Attach(t);
                TheInstrument.Tags.Add(t);
            }

            ContinuousFuture tmpCF = null;

            if (_addingNew)
            {
                if (TheInstrument.Exchange != null) _context.Exchanges.Attach(TheInstrument.Exchange);
                if (TheInstrument.PrimaryExchange != null) _context.Exchanges.Attach(TheInstrument.PrimaryExchange);
                _context.Datasources.Attach(TheInstrument.Datasource);

                if (TheInstrument.IsContinuousFuture)
                {
                    tmpCF = TheInstrument.ContinuousFuture; //EF can't handle circular references, so we hack around it
                    TheInstrument.ContinuousFuture = null;
                    TheInstrument.ContinuousFutureID = null;
                }
                _context.Instruments.Add(TheInstrument);
            }
            else //simply manipulating an existing instrument
            {
                //make sure any "loose" sessions are deleted
                if (!_addingNew)
                {
                    foreach (InstrumentSession s in _originalSessions.Where(s => !TheInstrument.Sessions.Any(x => x.ID == s.ID)))
                    {
                        _context.InstrumentSessions.Remove(s);
                    }
                }
            }

            _context.Database.Connection.Open();
            _context.SaveChanges();

            if (tmpCF != null)
            {
                _context.UnderlyingSymbols.Attach(tmpCF.UnderlyingSymbol);

                TheInstrument.ContinuousFuture = tmpCF;
                TheInstrument.ContinuousFuture.Instrument = TheInstrument;
                TheInstrument.ContinuousFuture.InstrumentID = TheInstrument.ID.Value;
                _context.SaveChanges();
            }

            InstrumentAdded = true;
            Hide();
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
                TheInstrument.OptionType = (OptionType?)OptionTypeComboBox.SelectedItem;
            }
            else
            {
                TheInstrument.OptionType = null;
            }
        }

        private void ExchangeRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (TheInstrument.Exchange == null) return;
            if (TheInstrument.SessionsSource == SessionsSource.Exchange) return; //we don't want to re-load them if it's already set

            SelectedSessions.Clear();
            TheInstrument.SessionsSource = SessionsSource.Exchange;

            foreach (ExchangeSession s in TheInstrument.Exchange.Sessions)
            {
                SelectedSessions.Add(s.ToInstrumentSession());
            }
        }

        private void CustomRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            TheInstrument.SessionsSource = SessionsSource.Custom;
        }

        private void TemplateRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            TheInstrument.SessionsSource = SessionsSource.Template;
            FillSessionsFromTemplate();
        }

        private void TemplateComboBox_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            FillSessionsFromTemplate();
        }

        private void FillSessionsFromTemplate()
        {
            SelectedSessions.Clear();

            var template = (SessionTemplate)TemplateComboBox.SelectedItem;
            if (template == null)
            {
                TheInstrument.SessionTemplateID = -1; //we can check for this later and deny the new instrument if its sessions are not set properly
                return;
            }

            TheInstrument.SessionTemplateID = template.ID;
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
            if (TheInstrument.SessionsSource == SessionsSource.Exchange)
            {
                SelectedSessions.Clear();

                foreach (ExchangeSession s in TheInstrument.Exchange.Sessions)
                {
                    SelectedSessions.Add(s.ToInstrumentSession());
                }
            }
        }

        private void RolloverRule_Checked(object sender, RoutedEventArgs e)
        {
            TheInstrument.ContinuousFuture.RolloverType = (ContinuousFuturesRolloverType)RolloverRuleType.SelectedItem;
        }

        private void RolloverTime_Checked(object sender, RoutedEventArgs e)
        {
            TheInstrument.ContinuousFuture.RolloverType = ContinuousFuturesRolloverType.Time;
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