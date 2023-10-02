using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Shipment
    {
        [Key]
        public string ShipmentId { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusCategory { get; set; }
        public string? StatusMilestone { get; set; }
        public string? OriginCountryCode { get; set; }
        public string? DestinationCountryCode { get; set; }
        public Delivery Delivery { get; set; }
        public List<TrackingNumber>? TrackingNumbers { get; set; }
        public Recipient? Recipient { get; set; }
    }
}