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
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Instrument> Instruments { get; set; }

        private Instrument _selectedInstrument;
        public Instrument SelectedInstrument
        {
            get { return _selectedInstrument; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstrument, value); }
        }

        public MainViewModel()
        {
            using (var context = new QDMSDbContext())
            {
                var instruments = new InstrumentManager().FindInstruments(context);
                Instruments = new ObservableCollection<Instrument>(instruments);
            }
        }
    }
}
