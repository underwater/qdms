using System.ComponentModel.DataAnnotations;

namespace QDMSService.Config
{
    public class DataServiceConfig
    {
        [Required]
        public DatabaseConnectionConfig LocalStorage { get; set; }
        
        public InstrumentServiceConfig InstrumentService { get; set; }

        public HistoricalDataServiceConfig HistoricalDataService { get; set; }

        public RealtimeDataServiceConfig RealtimeDataService { get; set; }
    }
}
