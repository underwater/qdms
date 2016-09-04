using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMSServer.ViewModels
{
    public class BaseAddInstrumentViewModel<T> : BaseViewModel where T : class
    {
        protected const string ApiKey = "f8d71bdcf1d7153e157e0baef35f67db";
        private IList<T> _selectedSeries;
        private string _searchText;
        private string _status;
        private ObservableCollection<T> _items;

        public ObservableCollection<T> Items
        {
            get { return _items; }
            set { this.RaiseAndSetIfChanged(ref _items, value); }
        }

        public IList<T> SelectedItems
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

        public ReactiveCommand<IEnumerable<T>> SearchCommand { get; protected set; }

        public ReactiveCommand<object> AddCommand { get; protected set; }

    }
}
