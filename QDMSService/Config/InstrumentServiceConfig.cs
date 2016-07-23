using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace QDMSService.Config
{
    public class InstrumentServiceConfig
    {
        [XmlAttribute]
        [DefaultValue(5558)]
        [Range(1, 65535)]
        public int Port { get; set; }
    }
}
