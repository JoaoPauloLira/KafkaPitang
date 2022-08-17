namespace Producer;

public interface IKafkaDispatcher
{
    Task<CorrelationId> Dispatch<T>(CorrelationId id, string topic, T value, CancellationToken token = default);
}