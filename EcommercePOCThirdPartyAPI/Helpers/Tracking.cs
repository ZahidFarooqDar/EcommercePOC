using EcommercePOCThirdPartyAPI.DomainModals;
using Microsoft.Extensions.Logging;

namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class Tracking
    {
        public Tracker Tracker { get; set; }
        public Shipment Shipment { get; set; }
        public List<Event> Events { get; set; }
        public Statistics? Statistics { get; set; }
    }
}