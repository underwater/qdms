using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMS.Server.DataSources
{
    public class EODCodeResolver
    {
        public bool TryResolve(string instrumentSymbol, out EODCode code)
        {
            code = Resolve(instrumentSymbol);
            return code != null;
        }
        private EODCode Resolve(string instrumentSymbol)
        {
            switch (instrumentSymbol)
            {
                case "MSFT": return new EODCode("NASDAQ", "MSFT");
                case "GOOG": return new EODCode("NASDAQ", "GOOG");
                case "FB": return new EODCode("NASDAQ", "FB"); ;

                #region // US Scenario
                case "ALD": return new EODCode("LSE", "ALD");
                case "AO.": return new EODCode("LSE", "AO");
                case "BAG": return new EODCode("LSE", "BAG");
                case "BOK": return new EODCode("LSE", "BOK");
                case "BRW": return new EODCode("LSE", "BRW");
                case "CARD": return new EODCode("LSE", "CARD");
                case "CCC": return new EODCode("LSE", "CCC");
                case "CINE": return new EODCode("LSE", "CINE");
                case "CLI": return new EODCode("LSE", "CLI");
                case "DFS": return new EODCode("LSE", "DFS");
                case "DPH": return new EODCode("LSE", "DPH");
                case "ECM": return new EODCode("LSE", "ECM");
                case "EMG": return new EODCode("LSE", "EMG");
                case "FCSS": return new EODCode("LSE", "FCSS");
                case "FDSA": return new EODCode("LSE", "FDSA");
                case "FGP": return new EODCode("LSE", "FGP");
                case "FIF": return new EODCode("LSE", "FIF");
                case "FJVS": return new EODCode("LSE", "FJVS");
                case "FRI": return new EODCode("LSE", "FRI");
                case "GNC": return new EODCode("LSE", "GNC");
                case "GSS": return new EODCode("LSE", "GSS");
                case "HAS": return new EODCode("LSE", "HAS");
                case "HICL": return new EODCode("LSE", "HICL");
                case "HSX": return new EODCode("LSE", "HSX");
                case "IGG": return new EODCode("LSE", "IGG");
                case "INVP": return new EODCode("LSE", "INVP");
                case "JD.": return new EODCode("LSE", "JD");
                case "JE.": return new EODCode("LSE", "JE");
                case "JLT": return new EODCode("LSE", "JLT");
                case "JMG": return new EODCode("LSE", "JMG");
                case "JUP": return new EODCode("LSE", "JUP");
                case "KIE": return new EODCode("LSE", "KIE");
                case "LAD": return new EODCode("LSE", "LAD");
                case "MAB": return new EODCode("LSE", "MAB");
                case "MAI": return new EODCode("LSE", "MAI");
                case "MARS": return new EODCode("LSE", "MARS");
                case "MPI": return new EODCode("LSE", "MPI");
                case "MRC": return new EODCode("LSE", "MRC");
                case "MSLH": return new EODCode("LSE", "MSLH");
                case "NBLS": return new EODCode("LSE", "NBLS");
                case "NOG": return new EODCode("LSE", "NOG");
                case "PLND": return new EODCode("LSE", "PLND");
                case "PLP": return new EODCode("LSE", "PLP");
                case "PNL": return new EODCode("LSE", "PNL");
                case "POLY": return new EODCode("LSE", "POLY");
                case "PPB": return new EODCode("LSE", "PPB");
                case "QQ.": return new EODCode("LSE", "QQ");
                case "RAT": return new EODCode("LSE", "RAT");
                case "RSW": return new EODCode("LSE", "RSW");
                case "SAGA": return new EODCode("LSE", "SAGA");
                case "SCIN": return new EODCode("LSE", "SCIN");
                case "SGP": return new EODCode("LSE", "SGP");
                case "SMT": return new EODCode("LSE", "SMT");
                case "SVS": return new EODCode("LSE", "SVS");
                case "SXS": return new EODCode("LSE", "SXS");
                case "RMV": return new EODCode("LSE", "RMV");
                case "SHI": return new EODCode("LSE", "SHI");
                case "WEIR": return new EODCode("LSE", "WEIR");
                case "BNKR": return new EODCode("LSE", "BNKR");
                case "TATE": return new EODCode("LSE", "TATE");
                #endregion


                #region Currencies

                case "EURUSD": return new EODCode("FOREX", "EURUSD");
                case "CADUSD": return new EODCode("FOREX", "CADUSD");
                case "GBPUSD": return new EODCode("FOREX", "GBPUSD");
                case "CHFUSD": return new EODCode("FOREX", "CHFUSD");


                #endregion
                default:
                    return null;
                    //throw new ArgumentException($"No EODData lookup information is available for {instrumentSymbol}");
            }
        }

    }
}
