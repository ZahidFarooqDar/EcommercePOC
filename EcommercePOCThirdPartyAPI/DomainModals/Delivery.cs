using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Delivery
    {
        [Key]
        public string DeliveryId { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        [DefaultValue("")]
        public string? Service { get; set; }
        [DefaultValue("")]
        public string? SignedBy { get; set; }
    }
}
