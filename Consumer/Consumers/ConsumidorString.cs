using Confluent.Kafka;
using Entities.Enum;
using Microsoft.Extensions.Logging;

namespace Consumer;

public class ConsumidorString : KafkaBaseConsumer<string>
{
    public ConsumidorString(ILogger<KafkaBaseConsumer<string>> logger) : base(logger)
    {
    }

    public override Task Parse(Message<string, string> message)
    {
        var msg = message.Value;
        _logger.LogInformation($"******************************A msg consumida foi: {msg}");
        
        return Task.CompletedTask;
    }

    public override string GetTopic() => TopicosKafka.PITANG_STRING.ToString();

    public override string GetConsumerGroup() => nameof(ConsumidorString);
}