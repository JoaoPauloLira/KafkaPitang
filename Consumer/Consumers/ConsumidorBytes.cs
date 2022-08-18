using Confluent.Kafka;
using Entities;
using Entities.Enum;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer;

public class ConsumidorBytes : KafkaBaseConsumer<ArquivoUpload>
{
    public ConsumidorBytes(ILogger<KafkaBaseConsumer<ArquivoUpload>> logger) : base(logger)
    {
    }

    public override Task Parse(Message<string, ArquivoUpload> message)
    {
        var msg = message.Value;
        var arquivo = JsonConvert.SerializeObject(msg);
        _logger.LogInformation($"******************************A msg consumida foi: {arquivo}");
        
        return Task.CompletedTask;
    }

    public override string GetTopic() => TopicosKafka.PITANG_ARQUIVO.ToString();

    public override string GetConsumerGroup() => nameof(ConsumidorBytes);
}