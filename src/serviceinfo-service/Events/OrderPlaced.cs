namespace ServiceInfoService.Events
{
    public class OrderPlaced
    {
        public int OderId { get; set; }
        public string? Item { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsHealthCheck { get; set; }

        public OrderPlaced()
        {
            IsHealthCheck = false;
        }
    }
}