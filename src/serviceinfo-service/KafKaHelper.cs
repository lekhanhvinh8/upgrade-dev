


using Confluent.Kafka;

public class KafkaHelper
{
    public static Offset GetOffsetForTimestamp(string topicName, DateTime timestamp)
    {
        var config = new ConsumerConfig
        {
            GroupId = "serviceinfo-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
        {
            var topicPartition = new TopicPartition(topicName, new Partition(0));

            long targetTimestamp = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds();

            var timestampList = new List<TopicPartitionTimestamp>
            {
                new TopicPartitionTimestamp(topicPartition, new Timestamp(targetTimestamp, TimestampType.CreateTime))
            };

            // Retrieve the offsets for the specified timestamps
            var offsets = consumer.OffsetsForTimes(timestampList, TimeSpan.FromSeconds(5));

            // Get the offset for the specified partition
            var offset = offsets[0].Offset;

            return offset;

        }
    }

}