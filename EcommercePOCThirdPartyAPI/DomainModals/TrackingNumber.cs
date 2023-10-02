using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class TrackingNumber
    {
        [Key]
        public string TrackingNumberId { get; set; }
        public string? Tn { get; set; }
    }
}