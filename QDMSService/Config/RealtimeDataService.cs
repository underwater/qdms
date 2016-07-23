using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace QDMSService.Config
{
    public class RealtimeDataServiceConfig
    {
        [XmlAttribute]
        [DefaultValue(5556)]
        [Range(1, 65535)]
        public int RequestPort { get; set; }

        [XmlAttribute]
        [DefaultValue(5556)]
        [Range(1, 65535)]
        public int PublisherPort { get; set; }
    }
}
