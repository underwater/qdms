using QDMS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Instrument> Instruments { get; set; }
    }
}
