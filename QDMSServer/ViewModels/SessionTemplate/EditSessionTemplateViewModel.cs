using System;
using System.Collections.Generic;
using System.Linq;
using QDMS;
using ReactiveUI;
using EntityData;

namespace QDMSServer.ViewModels
{
    public class EditSessionTemplateViewModel : BaseViewModel
    {
        private const string titleForAdd = "Add new session template";
        private const string titleForModify = "Modify session template";

        private readonly SessionTemplate _originalSessionTemplate;
        private readonly QDMSDbContext _context = new QDMSDbContext();
        private TemplateSession _selectedSession;

        public TemplateSession SelectedSession
        {
            get { return _selectedSession; }
            set { this.RaiseAndSetIfChanged(ref _selectedSession, value); }
        }
        public SessionTemplatesViewModel SessionTemplatesViewModel
        {
            get;
            private set;
        }
        public SessionTemplate Template { get; set; }
        public ReactiveCommand<object> SaveCommand { get; set; }
        public ReactiveCommand<object> AddSessionCommand { get; set; }
        public ReactiveCommand<object> DeleteSessionCommand { get; set; }

        
        public EditSessionTemplateViewModel(SessionTemplatesViewModel sessionTemplatesViewModel)
        {
            SessionTemplatesViewModel = sessionTemplatesViewModel;

            if (SessionTemplatesViewModel.SelectedTemplate == null)
            {
                Title = titleForAdd;

                Template = new SessionTemplate()
                {
                    ID = -1,
                    Sessions = new List<TemplateSession>()
                };
            }
            else
            {
                Title = titleForModify;

                SessionTemplatesViewModel.SelectedTemplate.Sessions =
                     SessionTemplatesViewModel.SelectedTemplate.Sessions
                    .OrderBy(s => s.OpeningDay)
                    .ThenBy(s => s.OpeningTime)
                    .ToList();

                Template = SessionTemplatesViewModel.SelectedTemplate;
                _originalSessionTemplate = _context.SessionTemplates.Include("Sessions").Single(s => s.ID == Template.ID);
            }

            SaveCommand = ReactiveCommand.Create();

            SaveCommand.Subscribe(_ =>
                {
                    MyUtils.ValidateSessions(Template.Sessions.ToList<ISession>());
                    bool nameExists = _context.SessionTemplates.Any(x => x.Name == Template.Name);

                    if (nameExists && _originalSessionTemplate.Name != Template.Name)
                    {
                        MessageBus.Current.SendMessage("Name already exists, please change it.");
                        return;
                    }

                    if (Title == titleForAdd)
                    {
                        SessionTemplatesViewModel.Templates.Add(Template);
                    }
                    else
                    {
                        //_context.SessionTemplates.Attach(_originalSessionTemplate);
                        _context.Entry(_originalSessionTemplate).CurrentValues.SetValues(Template);
                        _context.SaveChanges();
                        foreach (var session in _originalSessionTemplate.Sessions.ToList())
                        {
                            if (!Template.Sessions.Any(s => s.ID == session.ID))
                                _context.TemplateSessions.Remove(session);
                            _context.SaveChanges();
                        }

                        foreach (var session in Template.Sessions)
                        {
                            var originalSession = _originalSessionTemplate.Sessions.SingleOrDefault(s => s.ID == session.ID);
                            if (originalSession != null)
                                // Update Sessions that are in the Template.Sessions collection
                                _context.Entry(originalSession).CurrentValues.SetValues(session);
                            else
                                // Insert Sessions into the database that are not
                                // in the Template.Sessions collection
                                _originalSessionTemplate.Sessions.Add(session);

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
            AddSessionCommand.Subscribe(_ =>
                {
                    var session = new TemplateSession() { IsSessionEnd = true };

                    if (Template.Sessions.Count == 0)
                    {
                        session.OpeningDay = DayOfTheWeek.Monday;
                        session.ClosingDay = DayOfTheWeek.Monday;
                    }
                    else
                    {
                        DayOfTheWeek maxDay = (DayOfTheWeek)Math.Min(6, Template.Sessions.Max(x => (int)x.OpeningDay) + 1);
                        session.OpeningDay = maxDay;
                        session.ClosingDay = maxDay;
                    }
                    Template.Sessions.Add(session);
                });


            DeleteSessionCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedSession, x => x.Value != null));
            DeleteSessionCommand.Subscribe(_ =>
                {
                    Template.Sessions.Remove(SelectedSession);
                });
        }

        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
