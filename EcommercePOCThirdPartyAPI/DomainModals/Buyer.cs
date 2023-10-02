using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Buyer
    {
        [Key]
        public string? BuyerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
