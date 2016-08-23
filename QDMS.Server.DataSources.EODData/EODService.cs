


using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using QDMS.Server.DataSources.EODData.DataSoapClient;

namespace QDMS.Server.DataSources
{

    /// <summary>
    /// http://ws.eoddata.com/data.asmx?wsdl
    /// </summary>
    public class EODService
    {
        private string _loginToken;
        private DataSoapClient _eodClient;


        public EODService()
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 524288;
            _eodClient = new DataSoapClient(
                binding,
                new EndpointAddress("http://ws.eoddata.com/data.asmx"));

            //Login();
        }


        private void Login()
        {
            var response = _eodClient.Login("advisor", "advisor");
            if (!string.IsNullOrEmpty(response.Message) && string.IsNullOrEmpty(response.Token))
            {
                var message = string.Format("EOD service error: {0}", response.Message);
                throw new Exception(message);
            }

            _loginToken = response.Token;
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }


        public async Task<IEnumerable<QUOTE>> GetPrices(string Exchange, string Symbol, DateTime startDate, DateTime endDate)
        {
            //If we get logging issue at first time, then re-logging into system
            if (string.IsNullOrEmpty(_loginToken))
                Login();
            //If we are getting it in seocnd time, then exit from method, because we can not get access to service 
            if (string.IsNullOrEmpty(_loginToken))
                throw new Exception("No login token");
            //return null;

            var response = await _eodClient.SymbolHistoryPeriodByDateRangeAsync(_loginToken, Exchange, Symbol,
                                                                            FormatDate(startDate), FormatDate(endDate), "d");

            if (response.QUOTES == null)
                throw new Exception( response.Message);

            return  response.QUOTES ;

        }

       
    }
}

