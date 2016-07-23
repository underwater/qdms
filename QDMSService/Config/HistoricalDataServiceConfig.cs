using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace QDMSService.Config
{
    public class HistoricalDataServiceConfig
    {
        [XmlAttribute]
        [DefaultValue(5555)]
        [Range(1, 65535)]
        public int Port { get; set; }
    }
}
