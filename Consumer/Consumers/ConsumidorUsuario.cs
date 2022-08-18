using Confluent.Kafka;
using Entities;
using Entities.Enum;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Consumer;

public class ConsumidorUsuario : KafkaBaseConsumer<Usuario>
{
    public ConsumidorUsuario(ILogger<KafkaBaseConsumer<Usuario>> logger) : base(logger)
    {
    }

    public override Task Parse(Message<string, Usuario> message)
    {
        var msg = message.Value;
        var usuario = JsonConvert.SerializeObject(msg);
        _logger.LogInformation($"******************************A msg consumida foi: {usuario}");
        
        return Task.CompletedTask;
    }

    public override string GetTopic() => TopicosKafka.PITANG_USUARIO.ToString();

    public override string GetConsumerGroup() => nameof(ConsumidorUsuario);
}