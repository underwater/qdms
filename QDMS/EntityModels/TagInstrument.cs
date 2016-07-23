using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDMS
{
    public class TagInstrument
    {
        public int InstrumentID { get; set; }
        public Instrument Instrument { get; set; }

        public int TagID { get; set; }
        public Tag Tag { get; set; }
    }
}
