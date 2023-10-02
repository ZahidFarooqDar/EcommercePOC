namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class CreateTrackerRequest
    {
        public string trackingNumber { get; set; }
        public string shipmentReference { get; set; }
        public string originCountryCode { get; set; }
        public string destinationCountryCode { get; set; }
        public string destinationPostCode { get; set; }
        public string shippingDate { get; set; }
        public List<string> courierCode { get; set; }
        public string courierName { get; set; }
        public string trackingUrl { get; set; }
        public string orderNumber { get; set; }
    }
}
