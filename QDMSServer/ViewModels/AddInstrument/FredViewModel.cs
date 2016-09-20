using EntityData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class FredViewModel : AddInstrumentBaseViewModel<FredUtils.FredSeries>
    {

        public MainViewModel MainViewModel { get; set; }

        public FredViewModel() : base()
        {
            var canSearch = this.WhenAny(x => x.SearchText, x => !string.IsNullOrEmpty(x.Value));
            SearchCommand = ReactiveCommand.CreateAsyncTask(canSearch, async _ =>
            {
                IsBusy = true;   
                Status = "Please wait searching contracts...";
                return await FredUtils.FindSeries(SearchText, ApiKey);
            })
            .OnExecuteCompleted(x =>
            {
                Items = new ObservableCollection<FredUtils.FredSeries>(x);               
                Status = Items?.Count + " contracts found";
                IsBusy = false;
            });
            
            AddCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedItems, x => x.Value != null));
            AddCommand.Subscribe(x =>
            {
                using (var context = new QDMSDbContext())
                {
                    int addedInstrumentCount = 0;
                    var instrumentSource = new InstrumentManager();
                    var fredDataSource = context.Datasources.FirstOrDefault(ds => ds.Name == "FRED");

                    foreach (var series in SelectedItems)
                    {
                        var instrument = FredUtils.SeriesToInstrument(series, fredDataSource);

                        if (instrumentSource.AddInstrument(instrument) != null)
                            addedInstrumentCount++;
                        MainViewModel?.Instruments.Add(instrument);
                    }
                    Status = string.Format("{0}/{1} instruments added.", addedInstrumentCount, SelectedItems.Count);
                }
            });
        }

    }
}
