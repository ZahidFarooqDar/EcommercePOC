namespace EcommercePOCThirdPartyAPI.TrackerResponseTR
{
    public class Tracker
    {
        public string TrackerId { get; set; }
        public string TrackingNumber { get; set; }
        public string ShipmentReference { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
