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
        private const string addTitle = "Add symbol";
        private const string modifyTitle = "Modify symbol";

        private QDMSDbContext _context;
        private readonly UnderlyingSymbol _originalSymbol;
       
        public RootSymbolsViewModel RootSymbolsViewModel { get; private set; }

        public UnderlyingSymbol Symbol { get; set; }

        public ReactiveCommand<object> SaveCommand { get; set; }

        private bool _dayBasedSymbol;
        public bool DayBasedSymbol
        {
            get { return _dayBasedSymbol; }
            set
            {
                this.RaiseAndSetIfChanged(ref _dayBasedSymbol, value);
                Symbol.Rule.ReferenceUsesDays = _dayBasedSymbol;
            }
        }

        private bool _weekBasedSymbol;
        public bool WeekBasedSymbol
        {
            get { return _weekBasedSymbol; }
            set
            {
                this.RaiseAndSetIfChanged(ref _weekBasedSymbol, value);
            }
        }

        private bool _lastBusinessDay;

        public bool LastBusinessDay
        {
            get { return _lastBusinessDay; }
            set
            {
                this.RaiseAndSetIfChanged(ref _lastBusinessDay, value);
                Symbol.Rule.ReferenceDayIsLastBusinessDayOfMonth = _lastBusinessDay;
            }
        }

        public EditRootSymbolsViewModel(RootSymbolsViewModel rootSymbolsViewModel)
        {
            _context = new QDMSDbContext();
            RootSymbolsViewModel = rootSymbolsViewModel;

            SaveCommand = ReactiveCommand.Create(this.WhenAny(x => x.Symbol.Symbol, x => !string.IsNullOrEmpty(x.Value)));
            SaveCommand.Subscribe(_ =>
            {
                //check that the symbol doesn't already exist
                bool symbolExists = _context.UnderlyingSymbols.Count(x => x.Symbol == Symbol.Symbol) > 0;
                bool addingNew = Symbol.ID == -1;

                if (symbolExists && addingNew)
                {
                    throw new ArgumentException("Must have a symbol.");
                }

                if (addingNew)
                {
                    _context.UnderlyingSymbols.Add(Symbol);
                    RootSymbolsViewModel.Symbols.Add(Symbol);
                }
                else
                {
                    _context.UnderlyingSymbols.Attach(_originalSymbol);
                    _context.Entry(_originalSymbol).CurrentValues.SetValues(Symbol);
                }
                _context.SaveChanges();
                CloseCommand.Execute(null);
            });

            if (RootSymbolsViewModel.SelectedSymbol == null)
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

            if (Symbol.Rule.ReferenceUsesDays)
                DayBasedSymbol = true;
            else if (Symbol.Rule.ReferenceDayIsLastBusinessDayOfMonth)
                LastBusinessDay = true;
            else
                WeekBasedSymbol = true;
        }

        public override void Dispose()
        {
            _context.Dispose();
            base.Dispose();
        }
    }
}
