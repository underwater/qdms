using QDMS.Server.DataSources.EODData.DataSoapClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMS.Server.DataSources
{
    public class EOD : HistoricalDataBase
    {
        private readonly string _password;
        private readonly string _user;
        private readonly EODService _service;
        private readonly EODCodeResolver _resolver;

        public EOD(string user, string pass) : base("EODData")
        {
            _user = user;
            _password = pass;
            _service = new EODService();
            _resolver = new EODCodeResolver();

        }
        //Downloads data from EODData
        public override List<OHLCBar> GetData(HistoricalDataRequest req)
        {
            var res = GetDataAsync(req.Instrument, req.StartingDate, req.EndingDate).Result.ToList();
            return res;
        }

        //Downloads data from EODData
        public async Task<IEnumerable<OHLCBar>> GetDataAsync(Instrument instrument, DateTime startDate, DateTime endDate)
        {
            IEnumerable<QUOTE> prices = null;
            IEnumerable<OHLCBar> result = Enumerable.Empty<OHLCBar>();
            var code = new EODCode(instrument.Exchange.Name, instrument.Symbol);
            prices = await _service.GetPrices(code.Exchange, code.TickerSymbol, startDate, endDate);

            if (prices == null)
                return Enumerable.Empty<OHLCBar>();

            result = prices
                        .Where(p => p.DateTime >= startDate && p.DateTime <= endDate)
                        .Select(p => new OHLCBar { DT = p.DateTime, Close = (decimal)p.Close });

            return result;
        }


    }
}
