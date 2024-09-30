namespace OrderServiceQuery.Core.Event
{
    public class EventModel
    {
        public string? Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? AggregateIdentifier { get; set; }
        public string? AggregateType { get; set; }
        public int Version { get; set; }
        public string? EventType { get; set; }
        public BaseEvent? EventData { get; set; }
    }
}