using EntityData;
using QDMS;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class AddInstrumentQuandlViewModel : BaseAddInstrumentViewModel<Instrument>
    {
        private readonly Datasource CurrentDatasource;
        private int _totalItemsCount;
        private int _itemsPerPage;
        private int _currentPage;

        public MainViewModel MainViewModel { get; set; }

        public QDMSDbContext EntityContext { get; private set; }

        public ObservableCollection<Exchange> Exchanges { get; set; }

        public ReactiveCommand<object> NextPageCommand { get; set; }

        public ReactiveCommand<object> PrevPageCommand { get; set; }

        public int TotalItemsCount
        {
            get { return _totalItemsCount; }
            set
            {
                this.RaiseAndSetIfChanged(ref _totalItemsCount, value);
                this.RaisePropertyChanged(nameof(TotalPage));
            }
        }

        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set
            {
                this.RaiseAndSetIfChanged(ref _itemsPerPage, value);
                this.RaisePropertyChanged(nameof(TotalPage));
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
        }

        public int TotalPage
        {
            get
            {
                if (TotalItemsCount != 0 && ItemsPerPage != 0)
                    return TotalItemsCount / ItemsPerPage;
                return 0;
            }
            set { }
        }

        public AddInstrumentQuandlViewModel() : base()
        {
            MainViewModel = Locator.Current.GetService<MainViewModel>();
            EntityContext = new QDMSDbContext();
            CurrentDatasource = EntityContext.Datasources.First(x => x.Name == "Quandl");
            Exchanges = new ObservableCollection<Exchange>(EntityContext.Exchanges);


            var canSearch = this.WhenAny(x => x.SearchText, x => !string.IsNullOrEmpty(x.Value));
            SearchCommand = ReactiveCommand.CreateAsyncTask(canSearch, async x =>
            {
                IsBusy = true;
                Status = "Please wait searching contracts...";
                return await QuandlUtils.FindInstruments(SearchText, CurrentPage);
            })
            .OnExecuteCompleted(x =>
            {
                Items = new ObservableCollection<Instrument>(x);
                ItemsPerPage = QuandlUtils.ItemsPerPage;
                TotalItemsCount = QuandlUtils.TotalItemsCount;
                Status = TotalItemsCount + " contracts found.";
                IsBusy = false;
            });

            var canNext = this.WhenAny(
                x => x.IsBusy, x => x.CurrentPage, x => x.TotalPage, (b, c, t) => !b.Value && c.Value < t.Value);

            NextPageCommand = ReactiveCommand.Create(canNext);
            NextPageCommand.Subscribe(_ =>
            {
                CurrentPage++;
                SearchCommand.Execute(null);
            });

            var canPrev = this.WhenAny(x => x.IsBusy, x => x.CurrentPage, (b, c) => !b.Value && c.Value > 0);
            PrevPageCommand = ReactiveCommand.Create(canPrev);
            PrevPageCommand.Subscribe(_ =>
            {
                CurrentPage--;
                SearchCommand.Execute(null);
            });

            AddCommand = ReactiveCommand.Create();
            AddCommand.Subscribe(_ =>
            {
                var instrumentSource = new InstrumentManager();
                int addedInstrumentCount = 0;
                foreach (Instrument instrument in SelectedItems)
                {
                    if (instrument.Exchange != null)
                        instrument.ExchangeID = instrument.Exchange.ID;
                    if (instrument.PrimaryExchange != null)
                        instrument.PrimaryExchangeID = instrument.PrimaryExchange.ID;

                    if(instrumentSource.AddInstrument(instrument) != null)
                        addedInstrumentCount++;
                }
                Status = string.Format("{0}/{1} instruments added.", addedInstrumentCount, SelectedItems.Count);
            });
        }

        public override void Dispose()
        {
            EntityContext?.Dispose();
            base.Dispose();
        }

    }
}
