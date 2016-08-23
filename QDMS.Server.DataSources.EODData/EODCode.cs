using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMS.Server.DataSources 
{
    public class EODCode
    {
        public string Exchange { get; set; }
        public string TickerSymbol { get; set; }

        public EODCode(string exchange, string tickerSymbol)

        {
            Exchange = exchange;
            TickerSymbol = tickerSymbol;
        }

    }
}
