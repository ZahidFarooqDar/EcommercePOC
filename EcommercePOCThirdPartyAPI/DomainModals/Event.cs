using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Event
    {
        [Key]
        public string? EventId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? EventTrackingNumber { get; set; }
        public string? Status { get; set; }
        public DateTime? OccurrenceDatetime { get; set; }
        public string? Order { get; set; }
        public string? Location { get; set; }
        public string? SourceCode { get; set; }
        public string? CourierCode { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusCategory { get; set; }
        public string? StatusMilestone { get; set; }
    }
}