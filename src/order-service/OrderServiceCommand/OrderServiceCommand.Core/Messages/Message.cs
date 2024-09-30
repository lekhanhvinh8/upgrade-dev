namespace OrderServiceCommand.Core.Messages
{
    public abstract class Message
    {
        public Message()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}