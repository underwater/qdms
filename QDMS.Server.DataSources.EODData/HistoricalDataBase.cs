using NLog;
using QDMS.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QDMS.Server.DataSources
{
    public abstract class HistoricalDataBase : IHistoricalDataSource

    {

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Thread _downloaderThread;
        private ConcurrentQueue<HistoricalDataRequest> _queuedRequests;
        private bool _runDownloader;

        public HistoricalDataBase()
        {
            _queuedRequests = new ConcurrentQueue<HistoricalDataRequest>();
            _downloaderThread = new Thread(DownloaderLoop);
        }

        public HistoricalDataBase(string name) : this()
        {
            Name = name;
        }

        private void DownloaderLoop()
        {
            HistoricalDataRequest req;
            while (_runDownloader)
            {
                while (_queuedRequests.TryDequeue(out req))
                {
                    RaiseEvent(HistoricalDataArrived, this, new HistoricalDataEventArgs(req, GetData(req)));
                }
                Thread.Sleep(15);
            }
        }

        

         /// <summary>
        /// Connect to the data source.
        /// </summary>
        public void Connect()
        {
            _runDownloader = true;
            _downloaderThread.Start();
        }

        /// <summary>
        /// Disconnect from the data source.
        /// </summary>
        public void Disconnect()
        {
            _runDownloader = false;
            _downloaderThread.Join();
        }
        /// <summary>
        /// Whether the connection to the data source is up or not.
        /// </summary>
        public bool Connected { get { return _runDownloader; } }


        /// <summary>
        /// The name of the data source.
        /// </summary>
        public string Name { get; private set; }


        public void RequestHistoricalData(HistoricalDataRequest request)
        {
            _queuedRequests.Enqueue(request);
        }

        //Downloads data from EODData
        public abstract List<OHLCBar> GetData(HistoricalDataRequest req);
        

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

      

        public event EventHandler<DataSourceDisconnectEventArgs> Disconnected;
        public event EventHandler<ErrorArgs> Error;
        public event EventHandler<HistoricalDataEventArgs> HistoricalDataArrived;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
