using EntityData;
using QDMS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels.RootSymbol
{
    public class RootSymbolsViewModel : BaseViewModel
    {
        private QDMSDbContext _context;
        public ReactiveList<UnderlyingSymbol> Symbols { get; set; }

        private UnderlyingSymbol _selectedSymbol;
        public UnderlyingSymbol SelectedSymbol
        {
            get { return _selectedSymbol; }
            set { this.RaiseAndSetIfChanged(ref _selectedSymbol, value); }
        }

        public ReactiveCommand<object> ModifyCommand { get; set; }

        public ReactiveCommand<object> AddCommand { get; set; }

        public ReactiveCommand<object> DeleteCommand { get; set; }

        public ReactiveCommand<object> ConfirmDeleteCommand { get; set; }

        public RootSymbolsViewModel()
        {
            _context = new QDMSDbContext();
            Symbols = new ReactiveList<UnderlyingSymbol>(_context.UnderlyingSymbols.OrderBy(s => s.Symbol));
            AddCommand = ReactiveCommand.Create();

            var canModify = this.WhenAny(x => x.SelectedSymbol, x => x.Value != null);
            ModifyCommand = ReactiveCommand.Create(canModify);
            DeleteCommand = ReactiveCommand.Create(canModify);

            DeleteCommand.Subscribe(_ =>
            {
                var instrumentCount = _context.Instruments.Count(x => x.SessionTemplateID == SelectedSymbol.ID && x.SessionsSource == SessionsSource.Template);
                if (instrumentCount > 0)
                {
                    MessageBus.Current.SendMessage(string.Format("Can't delete this template it has {0} instruments assigned to it.", instrumentCount));
                    throw new CommandAbortException();
                }
            });

            ConfirmDeleteCommand = ReactiveCommand.Create();
            ConfirmDeleteCommand.Subscribe(_ =>
            {
                Symbols.Remove(SelectedSymbol);
                _context.UnderlyingSymbols.Attach(SelectedSymbol);
                _context.UnderlyingSymbols.Remove(SelectedSymbol);
                _context.SaveChanges();
            });
        }

        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
