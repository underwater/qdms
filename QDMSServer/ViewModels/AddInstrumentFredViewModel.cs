using EntityData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class AddInstrumentFredViewModel : BaseViewModel
    {
        private const string ApiKey = "f8d71bdcf1d7153e157e0baef35f67db";
        private IList<FredUtils.FredSeries> _selectedSeries;
        private string _searchText;
        private string _status;

        public MainViewModel MainViewModel { get; set; }

        public ObservableCollection<FredUtils.FredSeries> Series { get; set; }

        public IList<FredUtils.FredSeries> SelectedSeries
        {
            get { return _selectedSeries; }
            set { this.RaiseAndSetIfChanged(ref _selectedSeries, value); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        public ReactiveCommand<IEnumerable<FredUtils.FredSeries>> SearchCommand { get; protected set; }

        public ReactiveCommand<object> AddCommand { get; protected set; }

        public AddInstrumentFredViewModel() : base()
        {
            var canSearch = this.WhenAnyValue(x => !string.IsNullOrEmpty(x.SearchText));
            SearchCommand = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>
            {
                IsBusy = true;
                Status = "Please wait searching contracts...";
                return await FredUtils.FindSeries(SearchText, ApiKey);
            })
            .OnExecuteCompleted(x =>
            {
                Series = new ObservableCollection<FredUtils.FredSeries>(x);
                IsBusy = false;
                Status = Series?.Count + " contracts found";
            });

            AddCommand = ReactiveCommand.Create(this.WhenAnyValue(x => x.SelectedSeries != null));
            AddCommand.Subscribe(x =>
            {
                using (var context = new MyDBContext())
                {
                    int addedInstrumentCount = 0;
                    var instrumentSource = new InstrumentManager();
                    var fredDataSource = context.Datasources.FirstOrDefault(ds => ds.Name == "FRED");

                    foreach (var series in SelectedSeries)
                    {
                        var instrument = FredUtils.SeriesToInstrument(series, fredDataSource);

                        if (instrumentSource.AddInstrument(instrument) != null)
                            addedInstrumentCount++;
                        MainViewModel?.Instruments.Add(instrument);
                    }
                    Status = string.Format("{0}/{1} instruments added.", addedInstrumentCount, SelectedSeries.Count);
                }
            });
        }

    }
}
