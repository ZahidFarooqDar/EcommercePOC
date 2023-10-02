namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class UpdateTrackerRequest
    {
        public bool IsSubscribed { get; set; }
        public string[]? CourierCode { get; set; }
    }
}
