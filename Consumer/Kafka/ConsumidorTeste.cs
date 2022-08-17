using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Consumer;

public class ConsumidorTeste : KafkaBaseConsumer<string>
{
    public ConsumidorTeste(ILogger<KafkaBaseConsumer<string>> logger) : base(logger)
    {
    }

    public override Task Parse(Message<string, string> message)
    {
        var msg = message.Value;
        _logger.LogInformation($"******************************A msg consumida foi: {msg}");
        
        return Task.CompletedTask;
    }

    public override string GetTopic() => "TesteJP";

    public override string GetConsumerGroup() => nameof(ConsumidorTeste);
}