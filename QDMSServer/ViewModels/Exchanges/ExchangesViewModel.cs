using EntityData;
using QDMS;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace QDMSServer.ViewModels.Exchanges
{
    public class ExchangesViewModel : BaseViewModel
    {
        private readonly QDMSDbContext _context;

        private readonly List<Exchange> _originalExchanges;


        private ReactiveList<Exchange> _exchanges;
        public ReactiveList<Exchange> Exchanges
        {
            get { return _exchanges; }
            set { this.RaiseAndSetIfChanged(ref _exchanges, value); }
        }

        private Exchange _selectedExchange;
        public Exchange SelectedExchange
        {
            get { return _selectedExchange; }
            set { this.RaiseAndSetIfChanged(ref _selectedExchange, value); }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public ReactiveCommand<object> Search { get; set; }

        public ReactiveCommand<object> ModifyCommand { get; set; }

        public ReactiveCommand<object> AddCommand { get; set; }

        public ReactiveCommand<object> DeleteCommand { get; set; }

        public ReactiveCommand<object> ConfirmDeleteCommand { get; set; }
        

        public ExchangesViewModel()
        {
            _context = new QDMSDbContext();
            _originalExchanges = _context.Exchanges.Include(nameof(Exchange.Sessions)).ToList();
            Exchanges = new ReactiveList<Exchange>(_originalExchanges.OrderBy(x => x.Name));

            var canExecute = this.WhenAny(x => x.SelectedExchange, x => x.Value != null);

            AddCommand = ReactiveCommand.Create();
            ModifyCommand = ReactiveCommand.Create(canExecute);
            DeleteCommand = ReactiveCommand.Create(canExecute);

            this.WhenAnyObservable(x => x.Exchanges.ItemsAdded).Subscribe(async item =>
            {
                IsBusy = true;
                _context.Exchanges.Add(item);
                await _context.SaveChangesAsync();
                IsBusy = false;
            });

            this.WhenAnyObservable(x => x.Exchanges.ItemsRemoved).Subscribe(async item =>
            {
                IsBusy = true;
                _context.Exchanges.Attach(item);
                _context.Exchanges.Remove(item);
                await _context.SaveChangesAsync();
                IsBusy = false;
            });


            ConfirmDeleteCommand = ReactiveCommand.Create();
            ConfirmDeleteCommand.Subscribe(item =>
            {
                Exchanges.Remove(SelectedExchange);
            });

            Search = ReactiveCommand.Create();
            Search.Subscribe(_ =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    Exchanges = new ReactiveList<Exchange>(_originalExchanges.OrderBy(x => x.Name));
                }
                else
                {
                    var searchText = SearchText.ToLower();
                    var filteredElements = _originalExchanges
                                            .Where(e => e.Name.ToLower().Contains(searchText) ||
                                                (e.LongName != null && e.LongName.ToLower().Contains(searchText)))
                                            .OrderBy(e => e.Name);
                    Exchanges = new ReactiveList<Exchange>(filteredElements);
                }
                
            });

            this.WhenAny(x => x.SearchText, x => x.Value)
                .Subscribe(text =>
                {
                    Search.Execute(null);
                });
        }
    }
}
