using EntityData;
using QDMS;
using QDMSServer.Helpers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public enum ShowErrorMessage
    {
        Visible,
        Hidden
    }

    public class ManualViewModel : BaseViewModel
    {
        private readonly List<InstrumentSession> _originalSessions;
        private QDMSDbContext _context;
        private QDMS.SessionTemplate _selectedTemplate;
        private bool _isEditMode;


        public MainViewModel MainViewModel { get; set; }

        public Instrument Instrument { get; set; }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { this.RaiseAndSetIfChanged(ref _isEditMode, value); }
        }

        public ObservableCollection<Exchange> Exchanges { get; set; }

        public ObservableCollection<QDMS.SessionTemplate> Templates { get; set; }

        public QDMS.SessionTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { this.RaiseAndSetIfChanged(ref _selectedTemplate, value); }
        }

        public ObservableCollection<InstrumentSession> Sessions { get; set; }

        private InstrumentSession _selectedSession;

        public InstrumentSession SelectedSession
        {
            get { return _selectedSession; }
            set { this.RaiseAndSetIfChanged(ref _selectedSession, value); }
        }

        public ObservableCollection<Datasource> Datasources { get; set; }

        public ObservableCollection<UnderlyingSymbol> UnderlyingSymbols { get; set; }

        public ObservableCollection<CheckableItem<Tag>> Tags { get; set; }

        public ObservableCollection<KeyValuePair<int, string>> ContractMonths { get; set; }

        public ReactiveCommand<object> AddCommand { get; set; }

        public ReactiveCommand<object> AddSessionCommand { get; set; }

        public ReactiveCommand<object> DeleteSessionCommand { get; set; }
        
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public ManualViewModel(bool isEditMode = true, bool clone = false, bool addingContFut = false)
        {
            MainViewModel = Locator.Current.GetService<MainViewModel>();
            _context = new QDMSDbContext();
            IsEditMode = isEditMode;

            Exchanges = new ObservableCollection<Exchange>(_context.Exchanges.OrderBy(x => x.Name));
            Templates = new ObservableCollection<QDMS.SessionTemplate>(_context.SessionTemplates.Include("Sessions").ToList());
            Datasources = new ObservableCollection<Datasource>(_context.Datasources);
            UnderlyingSymbols = new ObservableCollection<UnderlyingSymbol>(_context.UnderlyingSymbols);
            FillContractMonths();

            if (IsEditMode)
            {
                Instrument = MainViewModel?.SelectedInstrument;
                if (Instrument != null)
                {
                    if (Instrument.Exchange != null)
                        _context.Entry(Instrument.Exchange).Reload();

                    if (Instrument.ContinuousFuture != null)
                    {
                        _context.ContinuousFutures.Attach(Instrument.ContinuousFuture);
                        _context.Entry(Instrument.ContinuousFuture).Reload();
                    }

                    if (Instrument.Tags != null && Instrument.Tags.Any())
                    {
                        foreach (Tag tag in Instrument.Tags)
                        {
                            _context.Tags.Attach(tag);
                        }
                    }

                    if (Instrument.Sessions != null && Instrument.Sessions.Any())
                    {
                        foreach (InstrumentSession session in Instrument.Sessions)
                        {
                            _context.InstrumentSessions.Attach(session);
                        }
                    }

                    if (clone)
                        Instrument = (Instrument)Instrument.Clone();

                    if (Instrument.Tags == null)
                        Instrument.Tags = new List<Tag>();

                    if (Instrument.Sessions == null)
                        Instrument.Sessions = new List<InstrumentSession>();

                    Instrument.Sessions = Instrument.Sessions
                        .OrderBy(x => x.OpeningDay)
                        .ThenBy(x => x.OpeningTime)
                        .ToList();

                    _originalSessions = new List<InstrumentSession>(Instrument.Sessions);
                }
            }
            else
            {
                Instrument = new Instrument
                {
                    Tags = new List<Tag>(),
                    Sessions = new List<InstrumentSession>()
                };

                Instrument.SessionsSource = SessionsSource.Custom;


                //need to do some extra stuff if it's a continuous future
                if (addingContFut)
                {
                    Instrument.ContinuousFuture = new ContinuousFuture();
                    Instrument.Type = InstrumentType.Future;
                    Instrument.IsContinuousFuture = true;
                }

            }

            Sessions = new ObservableCollection<InstrumentSession>(Instrument.Sessions);
            Tags = new ObservableCollection<CheckableItem<Tag>>(_context.Tags.ToList().Select(t => new CheckableItem<Tag>(t, Instrument.Tags.Contains(t))));


            this.WhenAny(x => x.Instrument.SessionsSource, x => x.Value)
                                .Where(x => x == SessionsSource.Template)
                                .Do(x => FillSessionsFromTemplate())
                                .Subscribe(_ =>
                                {
                                    SelectedTemplate = Templates.FirstOrDefault(x => x.ID == Instrument.SessionTemplateID);
                                });

            this.WhenAny(x => x.SelectedTemplate, x => x.Value)
                .Where(t => t != null)
                .Subscribe(x =>
                {
                    FillSessionsFromTemplate();
                });

            this.WhenAny(x => x.Instrument.Exchange, x => x.Value)
                .Where(e => e != null && Instrument.SessionsSource == SessionsSource.Exchange)
                .Subscribe(_ => AddSessionFromInstrument());

            this.WhenAny(x => x.Instrument.SessionsSource, x => x.Value)
                .Where(source => Instrument.Exchange != null && source == SessionsSource.Exchange)
                .Subscribe(_ => AddSessionFromInstrument());

            AddCommand = ReactiveCommand.Create();
            AddCommand.Subscribe(_ => Add());

            AddSessionCommand = ReactiveCommand.Create();
            AddSessionCommand.Subscribe(_ =>
            {
                var toAdd = new InstrumentSession { IsSessionEnd = true };

                if (Sessions.Count == 0)
                {
                    toAdd.OpeningDay = DayOfTheWeek.Monday;
                    toAdd.ClosingDay = DayOfTheWeek.Monday;
                }
                else
                {
                    DayOfTheWeek maxDay = (DayOfTheWeek)Math.Min(6, Sessions.Max(x => (int)x.OpeningDay) + 1);
                    toAdd.OpeningDay = maxDay;
                    toAdd.ClosingDay = maxDay;
                }
                Sessions.Add(toAdd);
            });

            var canDeleteSession = this.WhenAny(x => x.SelectedSession, x => x.Value != null);
            DeleteSessionCommand = ReactiveCommand.Create(canDeleteSession);
            DeleteSessionCommand.Subscribe(_ =>
            {
                Sessions.Remove(SelectedSession);
            });
        }

        private void AddSessionFromInstrument()
        {
            Sessions.Clear();
            foreach (ExchangeSession s in Instrument.Exchange.Sessions)
            {
                Sessions.Add(s.ToInstrumentSession());
            }
        }

        public bool HasError { get; set; }

        private void Add()
        {
            HasError = false;
            MessageBus.Current.SendMessage(ShowErrorMessage.Hidden);
            if (!IsEditMode &&
                            _context.Instruments.Any(
                                x => x.DatasourceID == Instrument.DatasourceID &&
                                        x.ExchangeID == Instrument.ExchangeID &&
                                        x.Symbol == Instrument.Symbol &&
                                        x.Expiration == Instrument.Expiration)
                            )
            {

                //there's already an instrument with this key
                MessageBus.Current.SendMessage("Instrument already exists.Change datasource, exchange, or symbol.");
                return;
            }

            //check that if the user picked a template-based session set, he actually selected one of the templates
            if (Instrument.SessionsSource == SessionsSource.Template && Instrument.SessionTemplateID == -1)
            {
                MessageBus.Current.SendMessage("You must pick a session template.");
                HasError = true;
            }

            if (Instrument.IsContinuousFuture && Instrument.Type != InstrumentType.Future)
            {
                MessageBus.Current.SendMessage("Continuous futures type must be Future.");
                HasError = true;
            }

            if (Instrument.Datasource == null)
            {
                MessageBus.Current.SendMessage("You must select a data source.");
                HasError = true;
            }

            if (Instrument.Multiplier == null)
            {
                MessageBus.Current.SendMessage("Must have a multiplier value.");
                HasError = true;
            }

            //Validate the sessions
            if (Sessions.Count > 0)
            {
                try
                {
                    MyUtils.ValidateSessions(Sessions.ToList<ISession>());
                }
                catch (Exception ex)
                {
                    MessageBus.Current.SendMessage(ex.Message);
                    HasError = true;
                }
            }

            if (HasError)
                return;


            //move selected sessions to the instrument
            Instrument.Sessions.Clear();
            foreach (InstrumentSession s in Sessions)
            {
                //need to attach?
                Instrument.Sessions.Add(s);
            }

            Instrument.Tags.Clear();

            foreach (Tag t in Tags.Where(x => x.IsChecked).Select(x => x.Item))
            {
                _context.Tags.Attach(t);
                Instrument.Tags.Add(t);
            }

            ContinuousFuture tmpCF = null;

            if (!IsEditMode)
            {
                if (Instrument.Exchange != null) _context.Exchanges.Attach(Instrument.Exchange);
                if (Instrument.PrimaryExchange != null) _context.Exchanges.Attach(Instrument.PrimaryExchange);
                _context.Datasources.Attach(Instrument.Datasource);

                if (Instrument.IsContinuousFuture)
                {
                    tmpCF = Instrument.ContinuousFuture; //EF can't handle circular references, so we hack around it
                    Instrument.ContinuousFuture = null;
                    Instrument.ContinuousFutureID = null;
                }
                _context.Instruments.Add(Instrument);
            }
            else //simply manipulating an existing instrument
            {
                //make sure any "loose" sessions are deleted
                if (IsEditMode)
                {
                    foreach (InstrumentSession s in _originalSessions.Where(s => !Instrument.Sessions.Any(x => x.ID == s.ID)))
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

                Instrument.ContinuousFuture = tmpCF;
                Instrument.ContinuousFuture.Instrument = Instrument;
                Instrument.ContinuousFuture.InstrumentID = Instrument.ID.Value;
                _context.SaveChanges();
            }
            MainViewModel.Instruments.Add(Instrument);
            CloseCommand.Execute(null);
        }

        private void FillContractMonths()
        {
            ContractMonths = new ObservableCollection<KeyValuePair<int, string>>();
            //fill the continuous futures contrat month combobox
            for (int i = 1; i < 10; i++)
            {
                ContractMonths.Add(new KeyValuePair<int, string>(i, MyUtils.Ordinal(i) + " Contract"));
            }
        }

        public void FillSessionsFromTemplate()
        {
            Sessions.Clear();

            var template = SelectedTemplate;
            if (template == null)
            {
                Instrument.SessionTemplateID = -1; //we can check for this later and deny the new instrument if its sessions are not set properly
                return;
            }

            Instrument.SessionTemplateID = template.ID;
            foreach (TemplateSession s in template.Sessions.OrderBy(x => x.OpeningDay))
            {
                Sessions.Add(s.ToInstrumentSession());
            }
        }

        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
