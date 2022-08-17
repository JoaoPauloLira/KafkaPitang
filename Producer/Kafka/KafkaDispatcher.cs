using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Producer;

public class KafkaDispatcher : IKafkaDispatcher
{
    private readonly ILogger<KafkaDispatcher> _logger;

    public KafkaDispatcher(ILogger<KafkaDispatcher> logger)
    {
        _logger = logger;
    }

    public async Task<CorrelationId> Dispatch<T>(CorrelationId id, string topic, T value, CancellationToken token = default)
    {
        var configuracaoProducer = new ProducerConfig
        {
            BootstrapServers = "localhost:9092", //Local que o kafka esta executando
            EnableIdempotence = true, // Garante que a msg seja produzida apenas 1x e em ordem de produção, por padrão pede que seja  Acks.All e MessageSendMaxRetries = int.MaxValue
            Acks = Acks.All, // All = Bloqueia até que a msg seja confirmada por todas as replicas sincronizadas
            MessageSendMaxRetries = int.MaxValue, //int.MaxValue = 2147483647
            MessageMaxBytes = 22797510 //Tamanho máximo da mensagem de solicitação do protocolo Kafka
        };

        //Passa as configuração para o criar o producer e caso der erro podemos logar o erro
        using var producer = new ProducerBuilder<string, string>(configuracaoProducer)
            .SetValueSerializer(Serializers.Utf8)
            .SetLogHandler((p, logMessage) =>
            {
                if (logMessage.Level != SyslogLevel.Error) return;
                _logger.LogError($"Erro KafkaProducer: {logMessage.Message}");
            }).Build();

        var json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        var menssagem = new Message<string, string>
        {
            Key = id.Id,
            Value = json
        };

        try
        {
            var deliveryResult = await producer.ProduceAsync(topic, menssagem, token);
            _logger.LogInformation("Envio de mensagem '{0}' para a partição '{1}'", deliveryResult.Key, deliveryResult.Partition);
        }
        catch (Exception e)
        {
            _logger.LogError("Houve um erro ao enviar a informação para o tópico '{0}'", e.InnerException);
            _logger.LogError("Mensagem: '{0}'", e.Message);
        }
        
        return id;
    }
}