using Microsoft.EntityFrameworkCore;
using NLog;
using QDMS.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using EntityData;

namespace QDMS.Server.DataStorage.Sqlite
{
    public class SqliteStorage : IDataStorage
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private DbContextOptions _contextOptions;

        public SqliteStorage(DbContextOptions contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public void Connect()
        {
        }

        public void Disconnect()
        {
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
                if(_connected == value)
                    return;
                
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
            if (!instrument.ID.HasValue)
                throw new Exception("Instrument must have an ID assigned to it.");

            if (data.Count == 0)
            {
                Log(LogLevel.Error, "Local storage: asked to add data of 0 length");
                return;
            }

            throw new NotImplementedException();
        }

        public void AddDataAsync(OHLCBar data, Instrument instrument, BarSize frequency, bool overwrite = false)
        {
            AddData(new List<OHLCBar> { data }, instrument, frequency, overwrite);
        }

        public void AddDataAsync(List<OHLCBar> data, Instrument instrument, BarSize frequency, bool overwrite = false)
        {
            AddData(data, instrument, frequency, overwrite);
        }

        public void DeleteAllInstrumentData(Instrument instrument)
        {
            using (var context = new DataDBContext(_contextOptions))
            {
                context.Data.RemoveRange(context.Data.Where(d => d.InstrumentID == instrument.ID));
                context.StoredDataInfo.RemoveRange(context.StoredDataInfo.Where(i => i.InstrumentID == instrument.ID));
                context.SaveChanges();
            }

            Log(LogLevel.Info, string.Format("Deleted all data for instrument {0}", instrument));
        }

        public void DeleteData(Instrument instrument, BarSize frequency)
        {
            using (var context = new DataDBContext(_contextOptions))
            {
                context.Data.RemoveRange(context.Data.Where(d => d.InstrumentID == instrument.ID && d.Frequency == frequency));
                context.StoredDataInfo.RemoveRange(context.StoredDataInfo.Where(i => i.InstrumentID == instrument.ID && i.Frequency == frequency));
                context.SaveChanges();
            }

            Log(LogLevel.Info, string.Format("Deleted all {0} data for instrument {1}", frequency, instrument));
        }

        public void DeleteData(Instrument instrument, BarSize frequency, List<OHLCBar> bars)
        {
            using(var context = new DataDBContext(_contextOptions))
            {
                for (int i = 0; i < bars.Count; i++)
                {
                    DateTime queryTime = frequency < BarSize.OneDay
                        ? bars[i].DT
                        : bars[i].DT.Date; //for frequencies greater than a day, we don't care about time
                    
                    context.Data.RemoveRange(context.Data.Where(d => d.InstrumentID == instrument.ID && d.Frequency == frequency && d.DT == queryTime));
                }

                //check if there's any data left
                var count = (from d in context.Data
                            where d.InstrumentID == instrument.ID && d.Frequency == frequency
                            select d).Count();
                
                if (count == 0)
                {
                    //remove from the instrumentinfo table
                    context.StoredDataInfo.RemoveRange(context.StoredDataInfo.Where(i => i.InstrumentID == instrument.ID && i.Frequency == frequency));
                }
                else
                {
                    var infoEntries = (from i in context.StoredDataInfo
                                       where i.InstrumentID == instrument.ID && i.Frequency == frequency
                                       select i);
                    foreach (var info in infoEntries)
                    {
                        var dataQuery = context.Data.Where(d => d.InstrumentID == instrument.ID && d.Frequency == frequency); 
                        info.EarliestDate = dataQuery.Min(d => d.DT);
                        info.LatestDate = dataQuery.Max(d => d.DT);
                    }
                }

                context.SaveChanges();
            }

            Log(LogLevel.Info, string.Format("Deleted {0} {1} bars for instrument {2}", bars.Count, frequency, instrument));
        }

        public void Dispose()
        {
        }

        public List<OHLCBar> GetData(Instrument instrument, DateTime startDate, DateTime endDate, BarSize barSize = BarSize.OneDay)
        {
            using (var context = new DataDBContext(_contextOptions))
            {
                DateTime dtMin = barSize >= BarSize.OneDay ? startDate.Date : startDate;
                DateTime dtMax = barSize >= BarSize.OneDay ? endDate.Date : endDate;

                return (from d in context.Data
                        where d.InstrumentID == instrument.ID
                        && d.Frequency == barSize
                        && d.DT >= dtMin
                        && d.DT <= dtMax
                        orderby d.DT ascending
                        select d).ToList();
            }
        }

        public List<StoredDataInfo> GetStorageInfo(int instrumentID)
        {
            using(var context = new DataDBContext(_contextOptions))
            {
                return (from info in context.StoredDataInfo
                        where info.InstrumentID == instrumentID
                        select info).ToList();
            }
        }

        public StoredDataInfo GetStorageInfo(int instrumentID, BarSize barSize)
        {
            using(var context = new DataDBContext(_contextOptions))
            {
                return (from info in context.StoredDataInfo
                        where info.InstrumentID == instrumentID && info.Frequency == barSize
                        select info).FirstOrDefault();
            }
        }

        public void RequestHistoricalData(HistoricalDataRequest request)
        {
            var data = GetData(request.Instrument, request.StartingDate, request.EndingDate, request.Frequency);

            RaiseEvent(HistoricalDataArrived, this, new HistoricalDataEventArgs(request, data));
        }

        public void UpdateData(List<OHLCBar> data, Instrument instrument, BarSize frequency, bool adjust = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a message to the log.
        ///</summary>
        private void Log(LogLevel level, string message)
        {
            _logger.Log(level, message);
        }

        ///<summary>
        /// Raise the event in a threadsafe manner
        ///</summary>
        ///<param name="event"></param>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        ///<typeparam name="T"></typeparam>
        static private void RaiseEvent<T>(EventHandler<T> @event, object sender, T e)
        where T : EventArgs
        {
            EventHandler<T> handler = @event;
            if (handler == null) return;
            handler(sender, e);
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
