using NLog;
using QDMS.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QDMS.Server.DataStorage.Sqlite
{
    public class SqliteStorage : IDataStorage
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private string _connectionString;
        
        public SqliteStorage(string connectionString)
        {
            _connectionString = connectionString;
        }

        private bool _connected;

        /// <summary>
        /// Whether the connection to the data source is up or not.
        /// </summary>
        public bool Connected
        {
            get
            {
                return _connected;
            }

            set
            {
                _connected = value;
                OnPropertyChanged();
            }
        }

        public string Name => "Local Storage";

        public event EventHandler<DataSourceDisconnectEventArgs> Disconnected;
        public event EventHandler<ErrorArgs> Error;
        public event EventHandler<HistoricalDataEventArgs> HistoricalDataArrived;
        
        public void AddData(List<OHLCBar> data, Instrument instrument, BarSize frequency, bool overwrite = false, bool adjust = true)
        {
            throw new NotImplementedException();
        }

        public void AddDataAsync(OHLCBar data, Instrument instrument, BarSize frequency, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public void AddDataAsync(List<OHLCBar> data, Instrument instrument, BarSize frequency, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
        }

        public void DeleteAllInstrumentData(Instrument instrument)
        {
            throw new NotImplementedException();
        }

        public void DeleteData(Instrument instrument, BarSize frequency)
        {
            throw new NotImplementedException();
        }

        public void DeleteData(Instrument instrument, BarSize frequency, List<OHLCBar> bars)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
        }

        public void Dispose()
        {
        }

        public List<OHLCBar> GetData(Instrument instrument, DateTime startDate, DateTime endDate, BarSize barSize = BarSize.OneDay)
        {
            throw new NotImplementedException();
        }

        public List<StoredDataInfo> GetStorageInfo(int instrumentID)
        {
            throw new NotImplementedException();
        }

        public StoredDataInfo GetStorageInfo(int instrumentID, BarSize barSize)
        {
            throw new NotImplementedException();
        }

        public void RequestHistoricalData(HistoricalDataRequest request)
        {
            throw new NotImplementedException();
        }

        public void UpdateData(List<OHLCBar> data, Instrument instrument, BarSize frequency, bool adjust = false)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
