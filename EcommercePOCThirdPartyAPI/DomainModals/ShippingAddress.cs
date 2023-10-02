using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class ShippingAddress
    {
        [Key]
        public string? AddressId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? Subdivision { get; set; }

        // Define a foreign key property
        public string? BuyerId { get; set; }

        // Navigation property to the Buyer
        public Buyer? Buyer { get; set; }
    }
}
