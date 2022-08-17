using Confluent.Kafka;

namespace Consumer;

public interface IConsumerService<T>
{
    Task Parse(Message<string, T> message);
    string GetTopic();
    string GetConsumerGroup();
}