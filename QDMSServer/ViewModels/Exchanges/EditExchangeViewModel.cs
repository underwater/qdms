using QDMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using EntityData;

namespace QDMSServer.ViewModels.Exchanges
{
    public class EditExchangeViewModel : BaseViewModel
    {
        private const string titleForAdd = "Add new exchange";
        private const string titleForModify = "Modify exchange";

        private readonly Exchange _originalExchange;
        private readonly QDMSDbContext _context;
        private ExchangeSession _selectedSession;

        public ExchangeSession SelectedSession
        {
            get { return _selectedSession; }
            set { this.RaiseAndSetIfChanged(ref _selectedSession, value); }
        }
        public ExchangesViewModel ExchangesViewModel { get; set; }
        public Exchange Exchange { get; set; }
        public ReactiveList<TimeZoneInfo> TimeZones { get; set; }

        public ReactiveCommand<object> SaveCommand { get; set; }
        public ReactiveCommand<object> AddSessionCommand { get; set; }
        public ReactiveCommand<object> DeleteSessionCommand { get; set; }

        public EditExchangeViewModel(ExchangesViewModel exchangesViewModel)
        {
            _context = new QDMSDbContext();
            TimeZones = new ReactiveList<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones().ToList());
            ExchangesViewModel = exchangesViewModel;
            if (ExchangesViewModel.SelectedExchange == null)
            {
                Title = titleForAdd;
                Exchange = new Exchange { ID = -1 };
            }
            else
            {
                Title = titleForModify;
                Exchange = exchangesViewModel.SelectedExchange;
                _originalExchange = _context.Exchanges.Include(nameof(Exchange.Sessions)).Single(e => e.ID == Exchange.ID);
            }

            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(_ =>
            {
                MyUtils.ValidateSessions(Exchange.Sessions.ToList<ISession>());
                bool nameExists = _context.Exchanges.Any(x => x.Name == Exchange.Name);
                if (nameExists && _originalExchange?.Name != Exchange.Name)
                {
                    MessageBus.Current.SendMessage("Name already exists, please change it.");
                    return;
                }
                if (Title == titleForAdd)
                {
                    ExchangesViewModel.Exchanges.Add(Exchange);
                }
                else
                {
                    //_context.Exchanges.Attach(_originalExchange);
                    _context.Entry(_originalExchange).CurrentValues.SetValues(Exchange);
                    _context.SaveChanges();

                    foreach (var session in _originalExchange.Sessions.ToList())
                    {
                        if (!Exchange.Sessions.Any(s => s.ID == session.ID))
                            _context.ExchangeSessions.Remove(session);
                        _context.SaveChanges();
                    }
                    foreach (var exchangeSession in Exchange.Sessions)
                    {
                        var originalSession = _originalExchange.Sessions.SingleOrDefault(s => s.ID == exchangeSession.ID);
                        if (originalSession != null)
                            // Update Sessions that are in the Template.Sessions collection
                            _context.Entry(originalSession).CurrentValues.SetValues(exchangeSession);
                        else
                            // Insert Sessions into the database that are not
                            // in the Template.Sessions collection
                            _originalExchange.Sessions.Add(exchangeSession);
                        _context.SaveChanges();
                    }
                }
                CloseCommand.Execute(null);
            });

            SaveCommand.ThrownExceptions.Subscribe(ex =>
            {
                MessageBus.Current.SendMessage(ex);
            });

            AddSessionCommand = ReactiveCommand.Create();
            DeleteSessionCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedSession, x => x.Value != null));

            DeleteSessionCommand.Subscribe(_ =>
            {
                Exchange.Sessions.Remove(SelectedSession);
            });

            AddSessionCommand.Subscribe(_ =>
            {
                var session = new ExchangeSession { IsSessionEnd = true };
                if (!Exchange.Sessions.Any())
                {
                    session.OpeningDay = DayOfTheWeek.Monday;
                    session.ClosingDay = DayOfTheWeek.Monday;
                }
                else
                {
                    DayOfTheWeek maxDay = (DayOfTheWeek)Math.Min(6, Exchange.Sessions.Max(x => (int)x.OpeningDay) + 1);
                    session.OpeningDay = maxDay;
                    session.ClosingDay = maxDay;
                }
                Exchange.Sessions.Add(session);
            });
        }
    }
}
