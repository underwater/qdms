using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMS.Server.DataSources
{
    public static class EODUtils
    {
        public static IEnumerable<OHLCBar> Parse(EODData.DataSoapClient.QUOTE[] quotes)
        {
            throw new NotImplementedException();
        }
    }
}
