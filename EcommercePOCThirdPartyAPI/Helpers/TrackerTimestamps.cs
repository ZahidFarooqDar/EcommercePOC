namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class TrackerTimestamps
    {
        public DateTime? InfoReceivedDatetime { get; set; }
        public DateTime? InTransitDatetime { get; set; }
        public DateTime? OutForDeliveryDatetime { get; set; }
        public DateTime? FailedAttemptDatetime { get; set; }
        public DateTime? AvailableForPickupDatetime { get; set; }
        public DateTime? ExceptionDatetime { get; set; }
        public DateTime? DeliveredDatetime { get; set; }
    }
}