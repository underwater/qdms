using EntityData;
using QDMS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class SessionTemplatesViewModel : BaseViewModel
    {
        private readonly QDMSDbContext _context;
        
        private SessionTemplate _selectedTemplate;
        public SessionTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { this.RaiseAndSetIfChanged(ref _selectedTemplate, value); }
        }

        public ReactiveCommand<object> AddCommand { get; set; }

        public ReactiveCommand<object> ModifyCommand { get; set; }

        public ReactiveCommand<object> DeleteCommand { get; set; }

        public ReactiveCommand<object> ConfirmDeleteCommand { get; set; }

        public ReactiveList<SessionTemplate> Templates { get; set; }        

        public SessionTemplatesViewModel()
        {
            _context = new QDMSDbContext();
            Templates = new ReactiveList<SessionTemplate>(_context.SessionTemplates.Include("Sessions").OrderBy(s => s.Name));
            

            AddCommand = ReactiveCommand.Create();
            ModifyCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedTemplate, x => x.Value != null));
            DeleteCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedTemplate, x => x.Value != null));

            this.WhenAnyObservable(x => x.Templates.ItemsAdded).Subscribe(async item =>
            {
                IsBusy = true;
                _context.SessionTemplates.Add(item);
                await _context.SaveChangesAsync();
                IsBusy = false;
            });

            this.WhenAnyObservable(x => x.Templates.ItemsRemoved).Subscribe(async item => 
            {
                IsBusy = true;
                _context.SessionTemplates.Attach(item);
                _context.SessionTemplates.Remove(item);
                await _context.SaveChangesAsync();
                IsBusy = false;
            });

            ConfirmDeleteCommand = ReactiveCommand.Create();
            ConfirmDeleteCommand.Subscribe(_ =>
            {
                Templates.Remove(SelectedTemplate);
            });

        }
        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
