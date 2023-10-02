using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Product
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Define a foreign key property
        public string? SellerId { get; set; }

        // Navigation property to the Seller
        public Seller? Seller { get; set; }
    }
}
