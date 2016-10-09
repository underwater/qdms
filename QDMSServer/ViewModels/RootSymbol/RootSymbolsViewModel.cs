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
        QDMSDbContext _context;
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


            ConfirmDeleteCommand = ReactiveCommand.Create();
            ConfirmDeleteCommand.Subscribe(_ => 
            {
                Symbols.Remove(SelectedSymbol);
                _context.UnderlyingSymbols.Attach(SelectedSymbol);
                _context.UnderlyingSymbols.Remove(SelectedSymbol);
                _context.SaveChanges();
            });
        }
    }
}
