using System;
using System.Collections.Generic;
using NUnit.Framework;
using QDMS;
using QDMSServer;
using QDMS.Server.DataSources;

namespace QDMSTest
{
    [TestFixture]
    public class EODataTests
    {

        [Test]
        public void EODData_ConnectToServer()
        {

            var provider = new EOD("user", "pass");

            var msft = new Instrument();
            var req = new HistoricalDataRequest()
            {
                DataLocation = DataLocation.Both,
                StartingDate = new DateTime(2015, 1, 1),
                EndingDate = new DateTime(2016, 01, 01),
                Frequency = BarSize.OneDay,
                Instrument = msft
            };
            provider.Connect();
            provider.HistoricalDataArrived += Provider_HistoricalDataArrived;
            provider.RequestHistoricalData(req);
            
            

        }

        private void Provider_HistoricalDataArrived(object sender, HistoricalDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ExploreHostAClientConnectsToServer()
        {
            var _client = new QDMSClient.QDMSClient("testingclient", "127.0.0.1", 5553, 5554, 5555, 5556);
            var exchange = new Exchange() { ID = 1, Name = "NYSE", Sessions = new List<ExchangeSession>(), Timezone = "Eastern Standard Time" };
            var datasource = new Datasource() { ID = 1, Name = "Yahoo" };
            var msft = new Instrument() { Symbol = "MSFT", UnderlyingSymbol = "MSFT", Type = InstrumentType.Stock, Currency = "USD", Exchange = exchange, Datasource = datasource, Multiplier = 1 };


            var req = new HistoricalDataRequest()
            {
                DataLocation = DataLocation.Both,
                StartingDate = new DateTime(2015, 1, 1),
                EndingDate = new DateTime(2016, 01, 01),
                Frequency = BarSize.OneDay,
                Instrument = msft
            };
            _client.Connect();
            var result = _client.RequestHistoricalData(req);


        }
    }
}
