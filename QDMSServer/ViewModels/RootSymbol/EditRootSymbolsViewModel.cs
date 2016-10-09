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
    public class EditRootSymbolsViewModel : BaseViewModel
    {
        private QDMSDbContext _context;
        private readonly UnderlyingSymbol _originalSymbol;
        private const string addTitle = "Add symbol";
        private const string modifyTitle = "Modify symbol";

        public RootSymbolsViewModel RootSymbolsViewModel { get; private set; }
        
        public UnderlyingSymbol Symbol { get; set; }

        public ReactiveCommand<object> SaveCommand { get; set; }


        public EditRootSymbolsViewModel(RootSymbolsViewModel rootSymbolsViewModel)
        {
            _context = new QDMSDbContext();
            RootSymbolsViewModel = rootSymbolsViewModel;

            if(RootSymbolsViewModel.SelectedSymbol == null)
            {
                Title = addTitle;
                Symbol = new UnderlyingSymbol
                {
                    ID = -1,
                    Rule = new ExpirationRule()
                };
            }
            else
            {
                Title = modifyTitle;
                Symbol = RootSymbolsViewModel.SelectedSymbol;
                _originalSymbol = _context.UnderlyingSymbols.Single(s => s.ID == Symbol.ID);
            }
        }
    }
}
