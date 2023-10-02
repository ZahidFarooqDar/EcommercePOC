namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class BuyProductRequest
    {
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BuyerId { get; set; }
    }
}
