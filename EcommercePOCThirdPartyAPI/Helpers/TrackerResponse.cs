using EcommercePOCThirdPartyAPI.DomainModals;

namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class TrackerResponse
    {
        public string? TrackerId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? ShipmentReference { get; set; }

        public bool IsSubscribed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
