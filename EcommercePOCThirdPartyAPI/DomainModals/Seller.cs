using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Seller
    {
        [Key]
        public string? SellerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }

        /*// Define a foreign key property
        public string? BuyerId { get; set; }

        // Navigation property to the Buyer
        public Buyer? Buyer { get; set; }*/
    }
}
