namespace OrderServiceCommand.API.Events
{
    public class OrderPlaced
    {
        public int OderId { get; set; }
        public string? Item { get; set; }
        public DateTime OrderDate { get; set; }
    }
}