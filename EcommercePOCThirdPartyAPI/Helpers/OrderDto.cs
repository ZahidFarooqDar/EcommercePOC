namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class OrderDto
    {
        public string? Id { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? BuyerId { get; set; }
        public string? SellerId { get; set; }
        public string? TrackerId { get; set; }
    }
}
