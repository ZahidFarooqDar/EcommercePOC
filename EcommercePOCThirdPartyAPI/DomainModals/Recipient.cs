using System.ComponentModel.DataAnnotations;

namespace EcommercePOCThirdPartyAPI.DomainModals
{
    public class Recipient
    {
        [Key]
        public string RecipientId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? City { get; set; }
        public string? Subdivision { get; set; }
    }
}