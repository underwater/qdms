using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QDMS;
using ReactiveUI;
using EntityData;

namespace QDMSServer.ViewModels.SessionTemplate
{
    public class EditSessionTemplateViewModel : BaseViewModel
    {
        private readonly QDMS.SessionTemplate _originalSessionTemplate;
        private readonly QDMSDbContext _context;
        private TemplateSession _selectedSession;


        public ReactiveCommand<object> ModifyCommand { get; set; }

        public ReactiveCommand<object> AddCommand { get; set; }

        public ReactiveCommand<object> RemoveCommand { get; set; }

        public ReactiveCommand<object> AddSessionCommand { get; set; }

        public ReactiveCommand<object> DeleteSessionCommand { get; set; }

        public SessionTemplatesViewModel SessionTemplatesViewModel { get; private set; }

        public bool IsEdit { get; set; }

        public TemplateSession SelectedSession
        {
            get { return _selectedSession; }
            set { this.RaiseAndSetIfChanged(ref _selectedSession, value); }
        }

        public QDMS.SessionTemplate SelectedSessionTemplate { get; set; }


        public EditSessionTemplateViewModel(SessionTemplatesViewModel sessionTemplatesViewModel)
        {
            SessionTemplatesViewModel = sessionTemplatesViewModel;
            _context = new QDMSDbContext();

            if (SessionTemplatesViewModel.SelectedTemplate == null)
            {
                Title = "Add";

                SelectedSessionTemplate = new QDMS.SessionTemplate()
                {
                    ID = -1,
                    Sessions = new List<TemplateSession>()
                };
            }
            else
            {
                Title = "Modify";
                IsEdit = true;

                SessionTemplatesViewModel.SelectedTemplate.Sessions =
                     SessionTemplatesViewModel.SelectedTemplate.Sessions
                    .OrderBy(s => s.OpeningDay)
                    .ThenBy(s => s.OpeningTime)
                    .ToList();

                SelectedSessionTemplate = SessionTemplatesViewModel.SelectedTemplate;
                _originalSessionTemplate = _context.SessionTemplates.Include("Sessions").Single(s => s.ID == SelectedSessionTemplate.ID);
            }

            ModifyCommand = ReactiveCommand.Create();
            AddCommand = ReactiveCommand.Create();
            RemoveCommand = ReactiveCommand.Create();

            AddCommand.Subscribe(_ =>
            {
                if (!IsEdit)
                {
                    SessionTemplatesViewModel.Templates.Add(SelectedSessionTemplate);
                    CloseCommand.Execute(null);
                }
                else
                {
                    //ensure sessions don't overlap
                    try
                    {
                        MyUtils.ValidateSessions(SelectedSessionTemplate.Sessions.ToList<ISession>());
                    }
                    catch (Exception ex)
                    {
                        MessageBus.Current.SendMessage(ex);
                        return;
                    }


                    bool nameExists = _context.SessionTemplates.Any(x => x.Name == SelectedSessionTemplate.Name);

                    if (nameExists && _originalSessionTemplate.Name != SelectedSessionTemplate.Name)
                    {
                        MessageBus.Current.SendMessage("Name already exists, please change it.");
                        return;
                    }

                    _context.Entry(_originalSessionTemplate).CurrentValues.SetValues(SelectedSessionTemplate);

                    // Delete subFoos from database that are not in the newFoo.SubFoo collection
                    foreach (var session in _originalSessionTemplate.Sessions.ToList())
                        if (!SelectedSessionTemplate.Sessions.Any(s => s.ID == session.ID))
                            _context.TemplateSessions.Remove(session);

                    foreach (var session in SelectedSessionTemplate.Sessions)
                    {
                        var originalSession = _originalSessionTemplate.Sessions.SingleOrDefault(s => s.ID == session.ID);
                        if (originalSession != null)
                            // Update subFoos that are in the newFoo.SubFoo collection
                            _context.Entry(originalSession).CurrentValues.SetValues(session);
                        else
                            // Insert subFoos into the database that are not
                            // in the dbFoo.subFoo collection
                            _originalSessionTemplate.Sessions.Add(session);
                    }
                    _context.SaveChanges();
                    CloseCommand.Execute(null);
                }
            });

            AddSessionCommand = ReactiveCommand.Create();
            AddSessionCommand.Subscribe(_ =>
            {
                var toAdd = new TemplateSession();
                toAdd.IsSessionEnd = true;

                if (SelectedSessionTemplate.Sessions.Count == 0)
                {
                    toAdd.OpeningDay = DayOfTheWeek.Monday;
                    toAdd.ClosingDay = DayOfTheWeek.Monday;
                }
                else
                {
                    DayOfTheWeek maxDay = (DayOfTheWeek)Math.Min(6, SelectedSessionTemplate.Sessions.Max(x => (int)x.OpeningDay) + 1);
                    toAdd.OpeningDay = maxDay;
                    toAdd.ClosingDay = maxDay;
                }
                SelectedSessionTemplate.Sessions.Add(toAdd);
            });


            DeleteSessionCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedSession, x => x.Value != null));
            DeleteSessionCommand.Subscribe(_ =>
            {
                SelectedSessionTemplate.Sessions.Remove(SelectedSession);
            });
        }
        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
